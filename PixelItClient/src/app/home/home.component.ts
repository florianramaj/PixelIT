import { Component, OnInit } from '@angular/core';
import { QueueClient, QueueServiceClient, StorageSharedKeyCredential } from "@azure/storage-queue";
import { HttpClient } from '@angular/common/http';
import { Buffer } from 'buffer';
import { SignalrService } from '../services/signalr.service';
import { HttpHandlerService } from '../services/http-handler.service';
import { Guid } from 'guid-typescript';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  fileUpload: string = "";
  base4File: string = "";
  imageName: string ="";

  constructor(public signalRService: SignalrService, private http: HttpClient, private httpService: HttpHandlerService) { }

  ngOnInit(): void {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();   
    this.startHttpRequest();
  }

  private _startHttpRequest = () => {
    this.http.get('https://webapppixelitwatchdogapi.azurewebsites.net/api/image')
      .subscribe(res => {
        console.log(res);
      });
  };
  public get startHttpRequest() {
    return this._startHttpRequest;
  }
  public set startHttpRequest(value) {
    this._startHttpRequest = value;
  }

  handleFileInput(target: any){


    var file = target.files[0] as File;

    var image = new Image();
   // this.fileUpload = 'data:image/png;base64,eyJUYWciOm51bGwsIlBoeXNpY2FsRGltZW5zaW9uIjp7IklzRW1wdHkiOmZhbHNlLCJXaWR0aCI6NDE3LCJIZWlnaHQiOjYxNX0sIlNpemUiOnsiSXNFbXB0eSI6ZmFsc2UsIldpZHRoIjo0MTcsIkhlaWdodCI6NjE1fSwiV2lkdGgiOjQxNywiSGVpZ2h0Ijo2MTUsIkhvcml6b250YWxSZXNvbHV0aW9uIjo5NS45ODY1OTUsIlZlcnRpY2FsUmVzb2x1dGlvbiI6OTUuOTg2NTk1LCJGbGFncyI6Nzc4NDIsIlJhd0Zvcm1hdCI6eyJHdWlkIjoiYjk2YjNjYWYtMDcyOC0xMWQzLTlkN2ItMDAwMGY4MWVmMzJlIn0sIlBpeGVsRm9ybWF0IjoyNDk4NTcwLCJQcm9wZXJ0eUlkTGlzdCI6Wzc3MSw3NjksMjA3NTIsMjA3NTMsMjA3NTRdLCJQcm9wZXJ0eUl0ZW1zIjpbeyJJZCI6NzcxLCJMZW4iOjEsIlR5cGUiOjEsIlZhbHVlIjoiQUE9PSJ9LHsiSWQiOjc2OSwiTGVuIjo4LCJUeXBlIjo1LCJWYWx1ZSI6Im9JWUJBSSt4QUFBPSJ9LHsiSWQiOjIwNzUyLCJMZW4iOjEsIlR5cGUiOjEsIlZhbHVlIjoiQVE9PSJ9LHsiSWQiOjIwNzUzLCJMZW4iOjQsIlR5cGUiOjQsIlZhbHVlIjoid3c0QUFBPT0ifSx7IklkIjoyMDc1NCwiTGVuIjo0LCJUeXBlIjo0LCJWYWx1ZSI6Ind3NEFBQT09In1dLCJQYWxldHRlIjp7IkZsYWdzIjowLCJFbnRyaWVzIjpbXX0sIkZyYW1lRGltZW5zaW9uc0xpc3QiOlsiNzQ2MmRjODYtNjE4MC00YzdlLThlM2YtZWU3MzMzYTdhNDgzIl19';
    document.body.appendChild(image);


    if(file.type != "image/png"){
      alert("There are just the formats png or jpg allowed!");
      return;
    }
    console.log(file);
    var reader = new FileReader();
    reader.readAsDataURL(file);
    this.imageName = file.name;

    reader.onload = (x) => {
      this.fileUpload = <string>x.target?.result;
      const base64String = this.fileUpload
                .replace('data:', '')
                .replace(/^.+,/, '');

            this.base4File = base64String;
           // this.fileUpload = "data:image/png;base64,"+ base64String;
    }

  }

  pixelit(){
    var id = Guid.create().toString();
    this.httpService.PixelIt({id: id, name: this.imageName, stringBytes: this.base4File}).subscribe(x => console.log(x));
  }





}
