import { Component, OnInit } from '@angular/core';
import { QueueClient, QueueServiceClient, StorageSharedKeyCredential } from "@azure/storage-queue";
import { Buffer } from 'buffer';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  fileUpload: string = "";

  constructor() { }

  ngOnInit(): void {
    this.test();
  }

  handleFileInput(target: any){


    var file = target.files[0] as File;

    var image = new Image();
    this.fileUpload = 'data:image/png;base64,eyJUYWciOm51bGwsIlBoeXNpY2FsRGltZW5zaW9uIjp7IklzRW1wdHkiOmZhbHNlLCJXaWR0aCI6NDE3LCJIZWlnaHQiOjYxNX0sIlNpemUiOnsiSXNFbXB0eSI6ZmFsc2UsIldpZHRoIjo0MTcsIkhlaWdodCI6NjE1fSwiV2lkdGgiOjQxNywiSGVpZ2h0Ijo2MTUsIkhvcml6b250YWxSZXNvbHV0aW9uIjo5NS45ODY1OTUsIlZlcnRpY2FsUmVzb2x1dGlvbiI6OTUuOTg2NTk1LCJGbGFncyI6Nzc4NDIsIlJhd0Zvcm1hdCI6eyJHdWlkIjoiYjk2YjNjYWYtMDcyOC0xMWQzLTlkN2ItMDAwMGY4MWVmMzJlIn0sIlBpeGVsRm9ybWF0IjoyNDk4NTcwLCJQcm9wZXJ0eUlkTGlzdCI6Wzc3MSw3NjksMjA3NTIsMjA3NTMsMjA3NTRdLCJQcm9wZXJ0eUl0ZW1zIjpbeyJJZCI6NzcxLCJMZW4iOjEsIlR5cGUiOjEsIlZhbHVlIjoiQUE9PSJ9LHsiSWQiOjc2OSwiTGVuIjo4LCJUeXBlIjo1LCJWYWx1ZSI6Im9JWUJBSSt4QUFBPSJ9LHsiSWQiOjIwNzUyLCJMZW4iOjEsIlR5cGUiOjEsIlZhbHVlIjoiQVE9PSJ9LHsiSWQiOjIwNzUzLCJMZW4iOjQsIlR5cGUiOjQsIlZhbHVlIjoid3c0QUFBPT0ifSx7IklkIjoyMDc1NCwiTGVuIjo0LCJUeXBlIjo0LCJWYWx1ZSI6Ind3NEFBQT09In1dLCJQYWxldHRlIjp7IkZsYWdzIjowLCJFbnRyaWVzIjpbXX0sIkZyYW1lRGltZW5zaW9uc0xpc3QiOlsiNzQ2MmRjODYtNjE4MC00YzdlLThlM2YtZWU3MzMzYTdhNDgzIl19';
    document.body.appendChild(image);


    if(file.type != "image/png"){
      alert("There are just the formats png or jpg allowed!");
      return;
    }
    console.log(file);
    var reader = new FileReader();
    reader.readAsDataURL(file);

    reader.onload = (x) => {
      this.fileUpload = <string>x.target?.result;
      const base64String = this.fileUpload
                .replace('data:', '')
                .replace(/^.+,/, '');

            console.log(base64String);
            this.fileUpload = "data:image/png;base64,"+ base64String;
    }

  }

  test()
  {
 
    const account =  "";
    const accountKey =  "";
  
    const connectionString = "DefaultEndpointsProtocol=https;AccountName=pixelit;AccountKey=pCfLoUpdc2ku/a1XDfOfT4j8RWZNfVQjqwK/iD2HP08cxQK7WP2twUcH1bhXY0XRIUPdNzDGkoiX+AStCAPTLQ==;EndpointSuffix=core.windows.net";
    // Create a unique name for the queue
    const queueName = "pixelitqueue";
    
    console.log("Creating queue: ", queueName);
    
    // Instantiate a QueueServiceClient which will be used
    // to create a QueueClient and to list all the queues
    const queueServiceClient = new QueueClient(connectionString, queueName);
    


  }




}
