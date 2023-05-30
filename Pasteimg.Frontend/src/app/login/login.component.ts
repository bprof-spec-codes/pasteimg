import { Component } from '@angular/core';
import { sessionIdService } from '../sessionId.service';
import { LoginModel } from '../_models/loginModel';
import { FormControl, Validators } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

    }
    return '';
  }


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
