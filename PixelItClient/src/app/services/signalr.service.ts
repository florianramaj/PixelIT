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

  public pixeldImage: string[] = [];
  public pixeldImageHelp: ImagePart[] = [];
  public pixelId: string = "";
  public imageName: string = "";
  public progressbar = 0;


  private hubConnection!: signalR.HubConnection;
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://pixelitwatchdogapi.azurewebsites.net/image', {
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
      this.httpService.SaveImage({id: data.identificator, name: new Date().toISOString(), stringBytes: data.stringBytes, imageId: data.imageId}).subscribe( x => console.log(x));

      if (data.imageId != this.pixelId)
        return;

      var progressElem = document.getElementById("progressDiv");
      
      var procent = (data.partNumber / (data.totalPart+1)) * 100;

      for (let progressbar = 0; progressbar <= procent; progressbar++) {
        if(progressbar == 100)
        {
          progressElem!.style.backgroundColor = "green";
        }

        progressElem?.style.setProperty("width", progressbar+"%");
        
      }

      
    
      this.pixeldImageHelp.push(data);


      this.checkOrder(data);
    });


  }

  checkOrder(data: ImagePart)
  {

    if(data.totalPart + 1 == this.pixeldImageHelp.length)
    {
      // 
      for (let index = 0; index < this.pixeldImageHelp.length; index++) {
        var filtered = this.pixeldImageHelp.find(x => x.partNumber-1 == index);

        if(filtered == undefined)
        {console.warn("Failed");}

        this.pixeldImage.push("data:image/png;base64," + filtered?.stringBytes);
        
      }
    }
  }
}