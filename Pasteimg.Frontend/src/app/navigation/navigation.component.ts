import { Component } from '@angular/core';
import { UploadService } from "../upload.service";

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent {
  constructor(
    private uploadService: UploadService
  ) {}

  isLoggedIn: boolean = false;

  ngOnInit() {
    this.uploadService.isLoggedIn$.subscribe((isLoggedIn) => {
      this.isLoggedIn = isLoggedIn;
      // Handle other logic when isLoggedIn changes
    });
  }

  logout(): void {
    this.uploadService.updateIsLoggedIn(false);
    this.uploadService.sessionId = '';
    localStorage.setItem('sessionId', '');
    localStorage.setItem('expirationTime', '');
    localStorage.clear();
    sessionStorage.clear();
    
  }

}
