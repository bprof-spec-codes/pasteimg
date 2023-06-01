import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UploadService } from './upload.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Pasteimg.Frontend';
  sessionLenghtInMinutes = 10;
  constructor(
    private http: HttpClient,
    private uploadService: UploadService
  ) { }

  async ngOnInit(): Promise<void> {
    const areWeIn = await this.uploadService.checkSessionIsAdmin();
    if (areWeIn==false){
      this.getSessionId();
    }
    const currentId=localStorage.getItem('sessionId');
    if (currentId!=null){
      this.uploadService.sessionId = currentId;
    }   
    window.addEventListener('mousedown', this.updateSessionStorageExpiration.bind(this));
    this.uploadService.isLoggedIn$.subscribe(() => {
      this.getSessionId();   
    });
  }

  async updateSessionStorageExpiration(): Promise<void> {
    const expirationTime = sessionStorage.getItem('expirationTime');
    if (expirationTime) {
      const expirationTimeMilliseconds = parseInt(expirationTime, 10);
      const currentTimeMilliseconds = new Date().getTime();
      const isAdmin = await this.uploadService.checkSessionIsAdmin();

      if (expirationTimeMilliseconds <= currentTimeMilliseconds && isAdmin) {
        // Expiration time has already passed, clear session storage
        localStorage.setItem('sessionId', '');
        localStorage.setItem('expirationTime', '');
        localStorage.clear();
        sessionStorage.clear();
        this.uploadService.updateIsLoggedIn(false);
        this.fetchSessionId();
      } else {
        const newExpirationMilliseconds = currentTimeMilliseconds + 60*this.sessionLenghtInMinutes;
        sessionStorage.setItem('expirationTime', newExpirationMilliseconds.toString());
      }
    }
    else{
      this.getSessionId();
    }
  }

  getSessionId(): void {
    if (localStorage.getItem('sessionId')==null) {
      this.fetchSessionId();
    }
  }

  fetchSessionId(): void {
    fetch(this.uploadService.backendUrl + '/api/Public/CreateSessionKey')
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch session ID');
        }
        return response.text();
      })
      .then(sessionId => {
        this.uploadService.sessionId = sessionId;
        const expirationTimeSeconds = 60*this.sessionLenghtInMinutes; // Set expiration time to 10 minutes (600 seconds)
        const expirationTimeMilliseconds = expirationTimeSeconds * 1000;
        const currentTimeMilliseconds = new Date().getTime();
        const expirationMilliseconds = currentTimeMilliseconds + expirationTimeMilliseconds;
        localStorage.setItem('sessionId', sessionId);
        sessionStorage.setItem('expirationTime', expirationMilliseconds.toString());
      })
      .catch(error => {
        console.error('Error fetching session ID:', error);
      });
  }

  async checkSessionIsAdmin(): Promise<boolean> {
    const sessionId = localStorage.getItem('sessionId');
    if (!sessionId) {
      throw new Error('Session ID not found');
    }

    const url = `${this.uploadService.backendUrl}/api/Admin/IsAdmin`;
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
      return isAdmin;
    } catch (error) {
      console.error('Error checking session isAdmin:', error);
      return false;
    }
  }
}
