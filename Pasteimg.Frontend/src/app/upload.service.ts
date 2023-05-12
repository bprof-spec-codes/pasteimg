import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {parseJson} from "@angular/cli/src/utilities/json-file";
import {Observable} from "rxjs";

export type Image = {
  "content": any,
  "description":string,
  "id": string,
  "nsfw": boolean,
  "uploadId": string
}

export type Upload = {
  "images": Image[],
  "id": string,
  "password"?: string,
  "timeStamp": string
}

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  http: HttpClient
  constructor(http: HttpClient) {
    this.http =http}

  async getUpload(id: string):Promise<Upload>  {
    return await (await fetch("https://localhost:7063/api/Public/GetUpload/" + id)).json()
  }
//ok, most lyó
  async postUpload(){ //na ez vicces lesz :D több kép is lehet benne
    // Hát ideje eldönteni, hogy egyessével postolod, vagy egyszerre
    // nem tudom melyik az egyszerűbb. gondolom az egyesével az backenden bonyibb itt egyszerűbb, az egyszerre itt bonyibb backenden egyszerűbb
    // Yup :D - de lehet hogy egyszerű backenden többet fogadni, még nem próbáltam mosdószünet
  }

}

