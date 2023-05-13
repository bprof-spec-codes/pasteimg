import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit {
  title = 'Pasteimg.Frontend';
  sessionId: string = '';
  backendUrl: string = 'https://localhost:7063';
  
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getSessionId();
  }


//#region Session Codes
  getSessionId(): void {
    if (!this.sessionId) {
      this.fetchSessionId();
    }
  }

  fetchSessionId(): void {
    fetch(this.backendUrl+'/api/Public/CreateSessionKey')
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch session ID');
        }
        return response.text();
      })
      .then(sessionId => {
        this.sessionId = sessionId;
      })
      .catch(error => {
        console.error('Error fetching session ID:', error);
      });
  }
//#endregion
 


}