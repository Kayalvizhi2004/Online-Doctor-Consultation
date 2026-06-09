import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { ApiService } from '../../../../core/services/api.service';
import { environment } from '../../../../../environments/environment';

interface ChatSession {
  id: string;
  sessionId: string;
  patientName: string;
  doctorName: string;
  patientImage?: string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  messageCount: number;
  duration: string;
  createdAt: string;
  summary?: string;
}

@Component({
  selector: 'app-chat-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-history.component.html',
  styleUrls: ['./chat-history.component.scss']
})
export class ChatHistoryComponent implements OnInit {

  sessions: ChatSession[] = [];
  isLoading = false;
  searchQuery = '';
  selectedSession: ChatSession | null = null;
  showTranscriptModal = false;
  transcriptMessages: any[] = [];
  transcriptLoading = false;

  constructor(
    private http: HttpClient,
    private router: Router,
    private appointmentService: AppointmentService,
    private api: ApiService
  ) {}

  ngOnInit(): void {
    this.loadChatHistory();
  }

  loadChatHistory(): void {
    this.isLoading = true;
    this.appointmentService.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        // Map appointments to chat sessions (expect appointment has sessionId or id)
        this.sessions = list.map((a: any) => ({
          id: a.id || a.appointmentId || a.appointmentId,
          sessionId: a.sessionId || a.id || a.appointmentId,
          patientName: a.patientName || a.patient?.fullName || 'Patient',
          doctorName: a.doctorName || a.doctor?.fullName || 'Doctor',
          patientImage: a.patientImage || a.patient?.avatarUrl,
          appointmentDate: a.date || a.appointmentDate || a.createdAt,
          startTime: a.startTime || a.time,
          endTime: a.endTime || a.end,
          messageCount: a.messageCount || 0,
          duration: a.duration || a.sessionDuration || '',
          createdAt: a.createdAt || a.date
        })).sort((a: any, b: any) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load chat history', err);
        this.isLoading = false;
      }
    });
  }

  getFilteredSessions(): ChatSession[] {
    if (!this.searchQuery) return this.sessions;

    const query = this.searchQuery.toLowerCase();
    return this.sessions.filter(s =>
      s.patientName.toLowerCase().includes(query) ||
      s.doctorName.toLowerCase().includes(query)
    );
  }

  viewSessionMessages(session: ChatSession): void {
    this.selectedSession = session;
    this.loadTranscript(session.sessionId);
    this.showTranscriptModal = true;
  }

  loadTranscript(sessionId: string): void {
    this.transcriptLoading = true;
    this.api.get<any>(`/api/sessions/${sessionId}/messages`, { pageNumber: 1, pageSize: 200 }).subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.data ?? res?.Data ?? []);
        this.transcriptMessages = list;
        this.transcriptLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load transcript', err);
        alert('Failed to load transcript');
        this.transcriptLoading = false;
      }
    });
  }

  closeTranscriptModal(): void {
    this.showTranscriptModal = false;
    this.selectedSession = null;
    this.transcriptMessages = [];
  }

  getDurationDisplay(duration: string): string {
    // Parse duration like "1h 30m" or convert from milliseconds
    return duration || '--';
  }

  downloadTranscript(sessionId: string): void {
    // Use full backend URL to avoid dev-server proxy returning index.html
    const url = `${environment.apiUrl}/api/sessions/${sessionId}/messages/transcript`;
    this.http.get(url, { responseType: 'blob' as 'json' }).subscribe({
      next: (blob: any) => {
        const objectUrl = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = objectUrl;
        link.download = `transcript-${sessionId}.txt`;
        link.click();
        window.URL.revokeObjectURL(objectUrl);
      },
      error: (err) => console.error('Failed to download transcript', err)
    });
  }

  exportSession(sessionId: string): void {
    const session = this.sessions.find(s => s.sessionId === sessionId);
    if (!session) return;

    const data = {
      sessionId: session.sessionId,
      patient: session.patientName,
      doctor: session.doctorName,
      date: session.appointmentDate,
      duration: session.duration,
      messages: session.messageCount,
      timestamp: new Date().toISOString()
    };

    const json = JSON.stringify(data, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `session-${sessionId}.json`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
