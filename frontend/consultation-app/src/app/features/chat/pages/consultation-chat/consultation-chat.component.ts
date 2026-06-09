import { Component, OnInit, OnDestroy, inject, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChatService } from '../../../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';

interface ChatMessage {
  id?: string;
  senderId: string;
  senderName?: string;
  message: string;
  messageType: 'text' | 'image' | 'gif'; // text, image, or gif
  content?: string; // base64 for images
  sentAt: string;
  status?: 'sent' | 'delivered' | 'read';
}

@Component({
  selector: 'app-consultation-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, TimeAgoPipe],
  templateUrl: './consultation-chat.component.html',
  styleUrls: ['./consultation-chat.component.scss']
})
export class ConsultationChatComponent implements OnInit, OnDestroy, AfterViewChecked {

  @ViewChild('scrollContainer') scrollContainer!: ElementRef;

  sessionId!: string;
  sessionTitle = 'Consultation';
  status: 'Active' | 'Ended' | 'Waiting' = 'Active';
  messages: ChatMessage[] = [];
  draft = '';
  me = localStorage.getItem('userId') || 'me';
  myRole = localStorage.getItem('userRole') || 'patient'; // 'doctor' or 'patient'
  
  isLoading = false;
  isSubmitting = false;
  isTyping = false;
  typingUsers: string[] = [];
  showImagePreview = false;
  previewImageUrl: string | null = null;
  selectedFile: File | null = null;
  showGifPicker = false;

  private route = inject(ActivatedRoute);
  private chatService = inject(ChatService);
  private router = inject(Router);
  private scrollToBottomOnNextCheck = false;

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId')!;
    this.loadMessageHistory();
    this.initializeSignalR();
  }

  ngAfterViewChecked(): void {
    if (this.scrollToBottomOnNextCheck) {
      this.scrollToBottom();
      this.scrollToBottomOnNextCheck = false;
    }
  }

  loadMessageHistory(): void {
    this.isLoading = true;
    this.chatService.getMessages(this.sessionId).subscribe({
      next: (res: any) => {
        this.messages = Array.isArray(res) ? res : (res?.items ?? res?.data ?? []);
        this.scrollToBottomOnNextCheck = true;
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load messages', err);
        this.isLoading = false;
      }
    });
  }

  initializeSignalR(): void {
    const token = localStorage.getItem('token') || '';
    this.chatService.initSignalR(token);
    this.chatService.joinSession(this.sessionId);

    // Subscribe to live messages
    this.chatService.messages$.subscribe((msgs: any[]) => {
      this.messages = msgs || [];
      this.scrollToBottomOnNextCheck = true;
    });

    // Subscribe to typing indicators
    this.chatService.typingUsers$.subscribe((users: string[]) => {
      this.typingUsers = users;
    });
  }

  send(): void {
    if (!this.draft || !this.draft.trim()) return;

    this.isSubmitting = true;
    this.chatService.sendRealtimeMessage(this.sessionId, this.draft, this.me).subscribe({
      next: () => {
        this.draft = '';
        this.isSubmitting = false;
        this.scrollToBottomOnNextCheck = true;
      },
      error: (err: any) => {
        console.error('Failed to send message', err);
        alert('Failed to send message. Please try again.');
        this.isSubmitting = false;
      }
    });
  }

  uploadImage(): void {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.onchange = (e: any) => {
      const file = e.target.files[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = (event: any) => {
          this.previewImageUrl = event.target.result;
          this.selectedFile = file;
          this.showImagePreview = true;
        };
        reader.readAsDataURL(file);
      }
    };
    input.click();
  }

  sendImage(): void {
    if (!this.selectedFile) return;

    this.isSubmitting = true;
    const reader = new FileReader();
    reader.onload = (e: any) => {
      const base64 = e.target.result;
      this.chatService.sendImageMessage(this.sessionId, base64, this.me).subscribe({
        next: () => {
          this.showImagePreview = false;
          this.previewImageUrl = null;
          this.selectedFile = null;
          this.isSubmitting = false;
          this.scrollToBottomOnNextCheck = true;
        },
        error: (err: any) => {
          console.error('Failed to send image', err);
          alert('Failed to send image. Please try again.');
          this.isSubmitting = false;
        }
      });
    };
    reader.readAsDataURL(this.selectedFile);
  }

  insertGif(gifUrl: string): void {
    this.isSubmitting = true;
    this.chatService.sendGifMessage(this.sessionId, gifUrl, this.me).subscribe({
      next: () => {
        this.showGifPicker = false;
        this.isSubmitting = false;
        this.scrollToBottomOnNextCheck = true;
      },
      error: (err: any) => {
        console.error('Failed to send GIF', err);
        alert('Failed to send GIF. Please try again.');
        this.isSubmitting = false;
      }
    });
  }

  notifyTyping(): void {
    this.chatService.notifyTyping(this.sessionId, this.me);
  }

  endSession(): void {
    if (!confirm('Are you sure you want to end this session?')) return;

    this.chatService.endSession(this.sessionId).subscribe({
      next: () => {
        this.status = 'Ended';
        alert('Session ended successfully');
        setTimeout(() => {
          this.router.navigate(['/chat-history']);
        }, 2000);
      },
      error: (err: any) => {
        console.error('Failed to end session', err);
        alert('Failed to end session. Please try again.');
      }
    });
  }

  cancelImagePreview(): void {
    this.showImagePreview = false;
    this.previewImageUrl = null;
    this.selectedFile = null;
  }

  scrollToBottom(): void {
    try {
      const container = this.scrollContainer.nativeElement;
      container.scrollTop = container.scrollHeight;
    } catch (err) {
      console.error('Failed to scroll to bottom', err);
    }
  }

  isMessageFromMe(message: ChatMessage): boolean {
    return message.senderId === this.me || message.senderId === localStorage.getItem('userId');
  }

  ngOnDestroy(): void {
    this.chatService.leaveSession(this.sessionId);
  }
}