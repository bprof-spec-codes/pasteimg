import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-image-password-check',
  templateUrl: './image-password-check.component.html',
  styleUrls: ['./image-password-check.component.scss']
})
export class ImagePasswordCheckComponent {

  router: ActivatedRoute
  snackBar: MatSnackBar
  nav: Router
  passwordError: FormControl
  http: HttpClient
  imageId: string = ''
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
      this.imageId = param['imageId']
    })
    

    this.header = new HttpHeaders({
      'Content-Type': 'application/json',
      //'SessionKeyHeader' : localStorage.getItem('sessionId')!.toString(),
      'API-SESSION-KEY' : localStorage.getItem('sessionId')!.toString()
    })  

    console.log(this.imageId);
    this.getUploadId()
    
  }

  private getUploadId(){
      this.http.get('https://localhost:7063/api/Public/GetImage/' + this.imageId, {headers: this.header}).subscribe(
        (success) => {
          
        },
        (error) => {
          console.log(error)
          this.uploadId = error.error.details.UploadId
          console.log(this.uploadId);
          
        })
  }

  public send(){
    this.http.post('https://localhost:7063/api/Public/EnterPassword/'+this.uploadId, JSON.stringify(this.password), {headers: this.header}).subscribe(
      (success) => {
        this.nav.navigate(['/link/image/'+this.imageId])
      },
      (error) => {
        console.log(error)
        this.snackBar.open(error.error.message, 'Close', { duration: 10000 })
      })
    }

}
