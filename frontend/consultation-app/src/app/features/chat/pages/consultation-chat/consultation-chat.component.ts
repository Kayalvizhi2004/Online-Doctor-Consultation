import { Component, OnInit, OnDestroy, inject, ViewChild, ElementRef, AfterViewChecked, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ChatService } from '../../../../core/services/chat.service';
import { AuthService } from '../../../../core/services/auth.service';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';
import { ToastrService } from 'ngx-toastr';

/** Implemented so the activeSessionGuard can block a patient from leaving. */
export interface CanComponentDeactivate {
  canDeactivate: () => boolean;
}

@Component({
  selector: 'app-consultation-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, TimeAgoPipe],
  templateUrl: './consultation-chat.component.html',
  styleUrls: ['./consultation-chat.component.scss']
})
export class ConsultationChatComponent implements OnInit, OnDestroy, AfterViewChecked, CanComponentDeactivate {

  @ViewChild('scrollContainer') scrollContainer!: ElementRef;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  sessionId!: string;
  status = signal<'Connecting' | 'Active' | 'Ended'>('Connecting');
  connError = signal<boolean>(false);
  messages = signal<any[]>([]);
  isSubmitting = signal<boolean>(false);
  isUploading = signal<boolean>(false);
  draft = '';

  private static readonly MAX_ATTACHMENT_BYTES = 5 * 1024 * 1024;
  private static readonly IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];

  me = '';
  myRole = '';
  /** Set true once the doctor ends the session — releases the leave-guard. */
  sessionEnded = false;

  private route = inject(ActivatedRoute);
  private chatService = inject(ChatService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private subs: Subscription[] = [];
  private scrollNext = false;
  private toastr = inject(ToastrService);
  showConfirm = false;
  confirmMessage = '';
  confirmAction: (() => void) | null = null;
  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId')!;

    this.auth.user$.subscribe(u => {
      this.me = u?.id || this.subFromToken();
      this.myRole = u?.role || '';
    });

    this.subs.push(this.chatService.messages$.subscribe(m => {
      this.messages.set(m);
      this.scrollNext = true;
    }));
    this.subs.push(this.chatService.sessionEnded$.subscribe(() => this.onSessionEnded()));

    this.chatService.init(this.sessionId)
      .then(() => this.status.set('Active'))
      .catch(() => this.connError.set(true));
  }

  isDoctor(): boolean { return this.myRole === 'Doctor'; }

  send(): void {
    const text = this.draft.trim();
    if (!text || this.status() === 'Ended') return;
    this.isSubmitting.set(true);
    this.chatService.sendMessage(this.sessionId, text).subscribe({
      next: () => { this.draft = ''; this.isSubmitting.set(false); },
      error: () => { this.isSubmitting.set(false); this.toastr.error('Message failed — the connection may have dropped.'); }
    });
  }

  attach(): void {
    if (this.status() === 'Ended' || this.isUploading()) return;
    this.fileInput?.nativeElement.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = ''; // allow re-selecting the same file
    if (!file || this.status() === 'Ended') return;

    if (!ConsultationChatComponent.IMAGE_TYPES.includes(file.type)) {
      this.toastr.info('Only JPEG, PNG, GIF and WebP images can be sent.');
      return;
    }
    if (file.size > ConsultationChatComponent.MAX_ATTACHMENT_BYTES) {
      this.toastr.info('Images must be 5 MB or smaller.');
      return;
    }

    this.isUploading.set(true);
    this.chatService.uploadAttachment(this.sessionId, file).subscribe({
      next: ({ url, messageType }) => {
        // Any typed text rides along as the caption.
        const caption = this.draft.trim();
        this.chatService.sendMessage(this.sessionId, caption, messageType, url).subscribe({
          next: () => { this.draft = ''; this.isUploading.set(false); },
          error: () => { this.isUploading.set(false); this.toastr.error('The image uploaded but sending failed — please try again.'); }
        });
      },
      error: () => { this.isUploading.set(false); this.toastr.error('Image upload failed. Please try again.'); }
    });
  }

endSession(): void {

  if (!this.isDoctor() || this.status() === 'Ended') {
    return;
  }

  this.confirmMessage = 'End this consultation for both you and the patient?';

  this.confirmAction = () => {

    this.chatService.endSession(this.sessionId).subscribe({
      next: () => {
        // The SessionEnded broadcast will redirect both users.
        this.showConfirm = false;
        this.confirmAction = null;
      },
      error: () => {
        this.toastr.error('Failed to end the session. Please try again.');
        this.showConfirm = false;
        this.confirmAction = null;
      }
    });

  };

  this.showConfirm = true;
}
confirmYes(): void {
  if (this.confirmAction) {
    this.confirmAction();
  }
}

confirmNo(): void {
  this.showConfirm = false;
  this.confirmAction = null;
}

  private onSessionEnded(): void {
    this.status.set('Ended');
    this.sessionEnded = true;
    setTimeout(() => {
      this.router.navigate([this.isDoctor() ? '/sessions' : '/appointments']);
    }, 1600);
  }

  isMine(m: any): boolean {
    return (m?.senderId || '') === this.me;
  }

  /** Guard hook: a patient may not leave while the session is still active. */
  canDeactivate(): boolean {
    if (this.sessionEnded || this.status() === 'Ended') return true;
    return this.isDoctor(); // doctors can leave; patients cannot
  }

  ngAfterViewChecked(): void {
    if (this.scrollNext) { this.scrollToBottom(); this.scrollNext = false; }
  }

  private scrollToBottom(): void {
    try {
      const c = this.scrollContainer?.nativeElement;
      if (c) c.scrollTop = c.scrollHeight;
    } catch { /* ignore */ }
  }

  private subFromToken(): string {
    try {
      const t = localStorage.getItem('token') || '';
      const payload = JSON.parse(atob(t.split('.')[1]));
      return payload.sub || payload.nameid || '';
    } catch { return ''; }
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
    this.chatService.leave(this.sessionId);
  }
}