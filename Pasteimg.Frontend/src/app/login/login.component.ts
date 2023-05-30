import { Component } from '@angular/core';
import { UploadService } from "../upload.service";
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  constructor(
    private router: Router,
    private uploadService: UploadService
  ) {}

  async ngOnInit(): Promise<void> {
    if (await this.uploadService.checkSessionIsAdmin()) 
    {
      this.router.navigate(['/upload']);
    }
  }

  email: string = "";
  password: string= "";

  async login(): Promise<void> {
    const isLoginSuccessfull = await this.uploadService.submitLogin(this.email, this.password);
    if (isLoginSuccessfull) {
      this.router.navigate(['/upload']);
    } else {
      // Handle login failure
      console.log('Login failed');
    }
  }
}
