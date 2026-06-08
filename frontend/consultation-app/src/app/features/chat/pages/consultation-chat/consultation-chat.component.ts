import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChatService } from '../../../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';

@Component({
  selector: 'app-consultation-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, TimeAgoPipe],
  templateUrl: './consultation-chat.component.html',
  styleUrls: ['./consultation-chat.component.scss']
})
export class ConsultationChatComponent implements OnInit, OnDestroy {

  sessionId!: string;
  sessionTitle = 'Consultation';
  status = 'Active';
  messages: any[] = [];
  draft = '';
  me = localStorage.getItem('userId') || 'me';

  private route = inject(ActivatedRoute);
  private chatService = inject(ChatService);
  private router = inject(Router);

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId')!;

    // Load history
    this.chatService.getMessages(this.sessionId)
      .subscribe((res: any) => this.messages = res || []);

    // SignalR init
    const token = localStorage.getItem('token') || '';
    this.chatService.initSignalR(token);

    this.chatService.joinSession(this.sessionId);

    // Subscribe live messages
    this.chatService.messages$.subscribe((msgs: any) => {
      this.messages = msgs || [];
    });
  }

  send(): void {
    if (!this.draft || !this.draft.trim()) return;
    this.chatService.sendRealtimeMessage(this.sessionId, this.draft, this.me);
    this.draft = '';
  }

  ngOnDestroy(): void {
    this.chatService.leaveSession(this.sessionId);
  }
}