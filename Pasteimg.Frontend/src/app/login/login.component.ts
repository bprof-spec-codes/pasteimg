import { Component } from '@angular/core';
import { LoginModel } from '../_models/loginModel';
import { FormControl, Validators } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { sessionIdService } from '../sessionId.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent {
  loginModel: LoginModel
  sessionIdService: sessionIdService
  email: FormControl
  password: FormControl
  http:HttpClient
  header: HttpHeaders
  snackBar: MatSnackBar
  nav: Router


  constructor(http: HttpClient, snackBar: MatSnackBar, nav: Router, idService: sessionIdService) {
    this.loginModel = new LoginModel()
    this.http = http
    this.snackBar = snackBar
    this.nav = nav
    this.sessionIdService = idService
    
    this.password = new FormControl('', [Validators.required])
    this.email = new FormControl('', [Validators.required, Validators.email])

    this.sessionIdService.getSessionId()

    this.header = new HttpHeaders({
      'Content-Type': 'application/json',
      //'SessionKeyHeader' : localStorage.getItem('sessionId')!.toString(),
      'API-SESSION-KEY' : localStorage.getItem('sessionId')!.toString()
    })  
  }

  public getPasswordErrorMessage() : string {
    if (this.password.hasError('required')) {
      return 'You must enter a password!';
    }
    return '';
  }

  public getEmailErrorMessage() : string {
    if (this.email.hasError('required')) {
      return 'You must enter a value!';
    }

    return this.email.hasError('email') ? 'Not a valid email!' : '';
  }

  public send(){
    console.log('sending');
    
    this.http.post('https://localhost:7063/api/Admin/Login', this.loginModel, {headers: this.header}).subscribe(
      (success) => {
        this.nav.navigate(['/admin'])
      },
      (error) => {
        console.log(error)
        this.snackBar.open(error.message, 'Close', { duration: 5000 })
      })
  }
}