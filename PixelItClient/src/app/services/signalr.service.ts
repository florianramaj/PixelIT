import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { Image } from '../models/Image';
import { ImagePart } from '../models/ImagePart';
import { HttpHandlerService } from './http-handler.service';


@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(private httpService: HttpHandlerService) {

  }

  pixeldImage: string[] = [];
  pixelId: string = "";
  imageName: string ="";
  progressbar = 0;


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
    this.hubConnection.on('ImageData', (data: ImagePart) => {
      console.log(data);
      this.httpService.SaveImage({id: data.identificator, name: this.imageName, stringBytes: data.stringBytes, imageId: data.imageId}).subscribe();

      if (data.imageId != this.pixelId)
        return;

      var test = document.getElementById("progressDiv");
      
      var procent = (data.partNumber / (data.totalPart+1)) * 100;

      for (let progressbar = 0; progressbar <= procent; progressbar++) {
        test?.style.setProperty("width", progressbar+"%");
        
      }

      
      //TODO: REIHENFOLGE BEACHTEN DU HEISL; zwischenlist 
      this.pixeldImage.push("data:image/png;base64," + data.stringBytes);
    });

    console.log("HEI");
  }
}