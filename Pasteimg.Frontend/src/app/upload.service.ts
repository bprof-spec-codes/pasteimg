import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { Observable, Subject } from 'rxjs';

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
  private isLoggedInSubject: Subject<boolean> = new Subject<boolean>();
  isLoggedIn$ = this.isLoggedInSubject.asObservable();
  isLoggedIn: boolean = false;
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

  async getImage(id: string):Promise<Image>{
    try{
      const response = await fetch(this.backendUrl+"/api/Public/GetImage/" + id);

      if (!response.ok) {
        if (response.status === 404) {
          let error = new Error('404 - File not Found');
          (error as any).status = response.status; // Add status to the error object
          throw error;
        }
        throw new Error('Network response was not ok');
      }

      const image = await response.json();

      return image;
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

  async submitLogin(email:string, password:string):Promise<Boolean> {
    const requestBody = {
      email: email,
      password: password
    };
  
    const response = await fetch(`${this.backendUrl}/api/Admin/Login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'API-SESSION-KEY': localStorage.getItem('sessionId')?.toString() || ''
      },
      body: JSON.stringify(requestBody)
    });
  
    if (response.ok) {
      const uploadResponseString = await response.text();
      console.log('LoginSuccesfull:', uploadResponseString);
      this.updateIsLoggedIn(true);
      return true;
    } else {
      console.error('LoginFailed:', response.status);
      return false;
    }
  }
  
  async checkSessionIsAdmin(): Promise<boolean> {
    const sessionId = localStorage.getItem('sessionId');
    if (!sessionId) {
      //throw new Error('Session ID not found');
      return false;
    }

    const url = `${this.backendUrl}/api/Admin/IsAdmin`;
    const headers = {
      'accept': '*/*',
      'API-SESSION-KEY': sessionId
    };

    try {
      const response = await fetch(url, { headers });
      if (!response.ok) {
        throw new Error('Failed to check session isAdmin');
      }
      const isAdmin = await response.json();
      this.updateIsLoggedIn(isAdmin);
      return isAdmin;
    } catch (error) {
      console.error('Error checking session isAdmin:', error);
      return false;
    }
  }

  updateIsLoggedIn(valueToChange: boolean) {
    this.isLoggedIn = valueToChange;
    this.isLoggedInSubject.next(valueToChange);
  }

  async editImage(imageId: string, description: string, nsfw: boolean): Promise<Boolean> {
    const requestBody = {
      description: description,
      nsfw: nsfw
    };
  
    const sessionId = localStorage.getItem('sessionId')  || '';
  
    const response = await fetch(`${this.backendUrl}/api/Admin/EditImage/${imageId}`, {
      method: 'PUT',
      headers: {
        'accept': '*/*',
        'API-SESSION-KEY': sessionId,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestBody)
    });
  
    if (response.ok) {
      console.log('Image edited successfully');
      return true;
    } else {
      console.error('Failed to edit image:', response.status);
      return false;
    }
  }

  async deleteImage(imageId: string): Promise<Boolean> {
    const sessionId = localStorage.getItem('sessionId') || '';
  
    const response = await fetch(`https://localhost:7063/api/Admin/DeleteImage/${imageId}`, {
      method: 'DELETE',
      headers: {
        'accept': '*/*',
        'API-SESSION-KEY': sessionId
      }
    });
  
    if (response.ok) {
      console.log('Image deleted successfully');
      return true;
    } else {
      console.error('Failed to delete image:', response.status);
      return false;
    }
  }
  
}

