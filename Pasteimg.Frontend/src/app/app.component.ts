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
  
  constructor(
    private http: HttpClient,
    private uploadService: UploadService
  ) { }

  ngOnInit(): void {
    this.getSessionId();
  }


//#region Session Codes
  getSessionId(): void {
    if (!this.uploadService.sessionId) {
      this.fetchSessionId();
    }
  }

  fetchSessionId(): void {
    fetch(this.uploadService.backendUrl+'/api/Public/CreateSessionKey')
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch session ID');
        }
        return response.text();
      })
      .then(sessionId => {
        this.uploadService.sessionId = sessionId;
      })
      .catch(error => {
        console.error('Error fetching session ID:', error);
      });
  }
//#endregion
 


}