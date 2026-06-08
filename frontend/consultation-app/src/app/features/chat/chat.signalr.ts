import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

export class ChatSignalR {

  private hubConnection!: signalR.HubConnection;

  startConnection(token: string): void {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.signalRUrl}/hubs/consultation`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Error', err));
  }

  getConnection(): signalR.HubConnection {
    return this.hubConnection;
  }

  stopConnection(): void {
    this.hubConnection?.stop();
  }
}