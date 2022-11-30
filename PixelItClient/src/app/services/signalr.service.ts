import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { Image } from '../models/Image';


@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor() {
    
   }

  public data!: Image;

  private hubConnection!: signalR.HubConnection;
    public startConnection = () => {
      this.hubConnection = new signalR.HubConnectionBuilder()
                              .withUrl('https://webapppixelitwatchdogapi.azurewebsites.net/image', {
                                skipNegotiation: true,
                                transport: signalR.HttpTransportType.WebSockets
                              })
                              .build();
      this.hubConnection
        .start()
        .then(() => console.log('Connection started'))
        .catch(err => console.log('Error while starting connection: ' + err))
    }
    
    public addTransferChartDataListener = () => {
      this.hubConnection.on('ImageData', (data) => {
        this.data = data;
        console.log(data);
      });
    }
}