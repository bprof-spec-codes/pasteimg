// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-register',
//   templateUrl: './register.component.html',
//   styleUrls: ['./register.component.scss']
// })
// export class RegisterComponent {

// }

import { Component } from '@angular/core';
import { sessionIdService } from '../sessionId.service';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { registerModel } from '../_models/registerModel';
import { FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  idService: sessionIdService
  registerModel: registerModel
  email: FormControl
  password: FormControl
  key: FormControl
  http:HttpClient
  header: HttpHeaders
  snackBar: MatSnackBar
  nav: Router


  constructor(idService: sessionIdService, http: HttpClient, snackBar: MatSnackBar, nav: Router) {
    this.idService = idService
    this.registerModel = new registerModel()
    this.http = http
    this.snackBar = snackBar
    this.nav = nav
    
    this.password = new FormControl('', [Validators.required])
    this.email = new FormControl('', [Validators.required, Validators.email])
    this.key = new FormControl('', [Validators.required])

    this.idService.getSessionId()

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

  public getKeyErrorMessage() : string {
    if (this.password.hasError('required')) {
      return 'You must enter a key!';
    }
    return '';
  }

  public send(){
    this.http.post('https://localhost:7063/api/Public/PostRegister', this.registerModel, {headers: this.header}).subscribe(
      (success) => {
        this.nav.navigate(['/login'])
      },
      (error) => {
        console.log(error)
        this.snackBar.open(error.error.message, 'Close', { duration: 5000 })
      })
  }
}
