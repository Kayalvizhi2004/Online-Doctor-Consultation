import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalRService {

  private hub!: signalR.HubConnection;

  startConnection() {
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl)
      .withAutomaticReconnect()
      .build();

    return this.hub.start();
  }

  joinSession(sessionId: string) {
    return this.hub.invoke('JoinSession', sessionId);
  }

  sendMessage(sessionId: string, message: string) {
    return this.hub.invoke('SendMessage', sessionId, message);
  }

  onMessage(callback: any) {
    this.hub.on('ReceiveMessage', callback);
  }
}