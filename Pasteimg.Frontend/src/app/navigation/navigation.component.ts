import { Component } from '@angular/core';
import { UploadService } from "../upload.service";
import { sessionIdService } from '../sessionId.service';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';


@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent {
  sessionIdService: sessionIdService
  http: HttpClient
  header: HttpHeaders
  nav: Router

  constructor(
    private uploadService: UploadService,
    sessionIdService: sessionIdService,
    http: HttpClient,
    nav: Router
  ) {
      this.header = new HttpHeaders
      this.sessionIdService = sessionIdService
      this.http = http
      this.nav = nav

      sessionIdService.getSessionId().then(id => {
        this.header = new HttpHeaders({
          //'SessionKeyHeader' : localStorage.getItem('sessionId')!.toString(),
          'API-SESSION-KEY' : id
        })
      })
      
      console.log(this.header)
      
      
  }

  isLoggedIn: boolean = false;

  ngOnInit() {
    this.uploadService.isLoggedIn$.subscribe((isLoggedIn) => {
      this.isLoggedIn = isLoggedIn;
      // Handle other logic when isLoggedIn changes
    });
  }

  logout(){
    // this.uploadService.updateIsLoggedIn(false);
    // this.uploadService.sessionId = '';
    // localStorage.setItem('sessionId', '');
    // localStorage.setItem('expirationTime', '');
    // localStorage.clear();
    // sessionStorage.clear();

    this.http.post('https://localhost:7063/api/Admin/Logout', {headers: this.header}, {headers: this.header}).subscribe(
      (success) => {
        this.sessionIdService.setAdmimFlag(false)
        this.nav.navigate(['/login'])
      },
      (error) => {
        console.log(error)
      })
    
  }

}
