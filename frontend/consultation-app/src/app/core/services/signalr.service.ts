import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalRService {

  private hub?: signalR.HubConnection;

  get connected(): boolean {
    return this.hub?.state === signalR.HubConnectionState.Connected;
  }

  startConnection(): Promise<void> {
    // The hub is [Authorize]; the backend reads the JWT from the
    // `access_token` query string for /hubs/consultation.
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl, {
        accessTokenFactory: () => localStorage.getItem('token') || ''
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    return this.hub.start();
  }

  joinSession(sessionId: string) {
    return this.hub?.invoke('JoinSession', sessionId);
  }

  leaveSession(sessionId: string) {
    return this.hub?.invoke('LeaveSession', sessionId);
  }

  sendMessage(sessionId: string, message: string, messageType?: string, attachmentUrl?: string) {
    // Allow sending optional metadata (type/attachment) — backend may accept these.
    // Keep signature flexible for older backends that only accept (sessionId, message).
    if (!this.hub) return undefined;
    if (messageType === undefined && attachmentUrl === undefined) {
      return this.hub.invoke('SendMessage', sessionId, message);
    }
    return this.hub.invoke('SendMessage', sessionId, message, messageType, attachmentUrl);
  }

  /** Doctor-only on the server; broadcasts SessionEnded to the room. */
  endSession(sessionId: string) {
    return this.hub?.invoke('EndSession', sessionId);
  }

  onMessage(cb: (msg: any) => void) {
    this.hub?.on('ReceiveMessage', cb);
  }

  onSessionEnded(cb: (payload: any) => void) {
    this.hub?.on('SessionEnded', cb);
  }

  onUserJoined(cb: (payload: any) => void) {
    this.hub?.on('UserJoined', cb);
  }

  stop() {
    const h = this.hub;
    this.hub = undefined;
    return h?.stop();
  }
}
