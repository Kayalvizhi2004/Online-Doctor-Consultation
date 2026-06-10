import { Component, OnInit, OnDestroy, inject, ViewChild, ElementRef, AfterViewChecked, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ChatService } from '../../../../core/services/chat.service';
import { AuthService } from '../../../../core/services/auth.service';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';

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

  sessionId!: string;
  status = signal<'Connecting' | 'Active' | 'Ended'>('Connecting');
  connError = signal<boolean>(false);
  messages = signal<any[]>([]);
  isSubmitting = signal<boolean>(false);
  draft = '';

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
      error: () => { this.isSubmitting.set(false); alert('Message failed — the connection may have dropped.'); }
    });
  }

  endSession(): void {
    if (!this.isDoctor() || this.status() === 'Ended') return;
    if (!confirm('End this consultation for both you and the patient?')) return;
    this.chatService.endSession(this.sessionId).subscribe({
      next: () => { /* the SessionEnded broadcast drives the redirect for everyone */ },
      error: () => alert('Failed to end the session. Please try again.')
    });
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
