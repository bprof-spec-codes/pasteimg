import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-upload-password-check',
  templateUrl: './upload-password-check.component.html',
  styleUrls: ['./upload-password-check.component.scss']
})
export class UploadPasswordCheckComponent {
  router: ActivatedRoute
  snackBar: MatSnackBar
  nav: Router
  passwordError: FormControl
  http: HttpClient
  uploadId: string = ''
  password: string = ''
  header: HttpHeaders
  
  constructor(router: ActivatedRoute, nav: Router, http: HttpClient, snackBar: MatSnackBar) {
    this.router = router
    this.nav = nav
    this.http = http
    this.snackBar = snackBar

    this.passwordError = new FormControl('', [Validators.required])

    this.router.params.subscribe(param => {
      this.uploadId = param['uploadId']
    })

    this.header = new HttpHeaders({
      'Content-Type': 'application/json',
      //'SessionKeyHeader' : localStorage.getItem('sessionId')!.toString(),
      'API-SESSION-KEY' : localStorage.getItem('sessionId')!.toString()
    })  

    console.log(this.uploadId);
    
  }

  public getPasswordErrorMessage() : string {
    if (this.passwordError.hasError('required')) {
      return 'You must enter a password!';
    }
    return '';
  }

  public send(){
    this.http.post('https://localhost:7063/api/Public/EnterPassword/'+this.uploadId, JSON.stringify(this.password), {headers: this.header}).subscribe(
      (success) => {
        this.nav.navigate(['/link/'+this.uploadId])
      },
      (error) => {
        console.log(error)
        this.snackBar.open(error.error.message, 'Close', { duration: 10000 })
      })
    }
}
