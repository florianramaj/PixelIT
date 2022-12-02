import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Image } from '../models/Image';

@Injectable({
  providedIn: 'root'
})
export class HttpHandlerService {

  constructor(private http: HttpClient) { }

  private endPoint = "https://pixelitapi.azurewebsites.net";

  private headers: HttpHeaders = new HttpHeaders().set("Content-Type", "application/json;");


  public PixelIt(image: Image)
  {
      return this.http.post<any>(this.endPoint + "/PixilateImage", image, {headers: this.headers});
  }

  public SaveImage(image: Image)
  {
    console.log(image);
    return this.http.post<any>(this.endPoint + "/Image", image, {headers: this.headers});
  }

  public GetImages()
  {
    return this.http.get<Image[]>(this.endPoint + "/Image");
  }
}
