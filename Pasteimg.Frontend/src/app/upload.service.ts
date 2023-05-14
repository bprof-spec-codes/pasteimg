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

export class Content {
  constructor(
    public contentType: string,
    public data: string,
    public fileName: string
  ) {}
}

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  http: HttpClient
  sessionId: string = '';
  backendUrl: string = 'https://localhost:7063';
  constructor(http: HttpClient) {
    this.http =http}

  async getUpload(id: string): Promise<Upload> {
    try {
      const response = await fetch(this.backendUrl+"/api/Public/GetUpload/" + id);
      
      if (!response.ok) {
        if (response.status === 404) {
          let error = new Error('404 - File not Found');
          (error as any).status = response.status; // Add status to the error object
          throw error;
        }
        throw new Error('Network response was not ok');
      }
      
      const upload = await response.json();
      
      return upload;
    } catch (error) {
      // Handle the error appropriately, such as logging or displaying a user-friendly message
      console.error('Error retrieving upload:', error);
      throw error; // Rethrow the error to propagate it to the caller
    }
  }

  async postUpload(postUpload: Upload): Promise<string> {
    console.log(JSON.stringify(postUpload));
   /*  const upload = new Upload(this.images, this.password); */   
      const response = await fetch(`${this.backendUrl}/api/Public/PostUpload`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(postUpload)
      });

      if (response.ok) {
        //Contains the id of the upload
        const uploadResponseString = await response.text();
        console.log('Upload successful:', uploadResponseString);
        return uploadResponseString;
      } else {
        console.error('Error uploading:', response.status);
        return "";
      }
  }

}

