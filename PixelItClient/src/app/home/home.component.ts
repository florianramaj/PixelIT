import { Component, OnInit } from '@angular/core';
import { QueueClient, QueueServiceClient, StorageSharedKeyCredential } from "@azure/storage-queue";
import { HttpClient } from '@angular/common/http';
import { Buffer } from 'buffer';
import { SignalrService } from '../services/signalr.service';
import { HttpHandlerService } from '../services/http-handler.service';
import { Guid } from 'guid-typescript';
import { merge, Observable, startWith, Subject, switchMap, tap } from 'rxjs';
import { Image } from '../models/Image';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  fileUpload: string = "";
  base4File: string = "";
  public reloadSubject: Subject<void> = new Subject();
  public images!: Observable<Image[]>;
  public localImages: Image[] = [];
  public isAllowed = false;



  constructor(public signalRService: SignalrService, private http: HttpClient, private httpService: HttpHandlerService) { }

  ngOnInit(): void {

    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();   
    this.startHttpRequest();
  }

  private _startHttpRequest = () => {
    this.http.get('https://pixelitwatchdogapi.azurewebsites.net/api/image')
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

    this.isAllowed = false;
    var file = target.files[0] as File;

    if(file.type != "image/png"){
      alert("There is just the format png allowed!");
      return;
    }
    this.isAllowed = true;
    console.log(file);
    var reader = new FileReader();
    reader.readAsDataURL(file);
    this.signalRService.imageName = file.name.toString();

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
    this.signalRService.pixeldImage = [];
    this.signalRService.pixeldImageHelp = [];
    var id = Guid.create().toString();
    this.signalRService.pixelId = id;
    this.signalRService.progressbar = 5;
    var progressElem = <HTMLInputElement>document.getElementById("progressDiv");
    progressElem?.style.setProperty("width", this.signalRService.progressbar+"%");
    progressElem!.style.backgroundColor = "#0d6efd";
    this.httpService.PixelIt({id: id, name: this.signalRService.imageName, stringBytes: this.base4File, imageId: Guid.EMPTY}).subscribe(x => console.log(x));
  }

  getImages(){
    this.images = merge(this.reloadSubject).pipe(
      startWith({}),
      switchMap(() => {
        return this.httpService.GetImages()
      }),
      tap(a => {
        a.every(x => x.stringBytes = "data:image/png;base64," + x.stringBytes);

        this.localImages = a;


      }),
    );

    this.images.subscribe();
    
  }





}
