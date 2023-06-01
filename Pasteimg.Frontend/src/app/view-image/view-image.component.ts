import { Component } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { Image, UploadService } from "../upload.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { sessionIdService } from "../sessionId.service";

@Component({
  selector: 'app-view-image',
  templateUrl: './view-image.component.html',
  styleUrls: ['./view-image.component.scss']
})
export class ViewImageComponent {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private uploadService: UploadService,
    private http: HttpClient,
    private sessionIdService: sessionIdService
  ) {}

  image!: Image;
  contentType: any;
  data: any;

  async ngOnInit() {
    const sessionKey = await this.sessionIdService.getSessionId();
    const routeParams = this.route.snapshot.paramMap;
    const imageIdFromRoute = String(routeParams.get('imageId'));

    this.uploadService.getImage(imageIdFromRoute).then(i => {
      this.image = i;
    })
    .catch(error => {
      if (error.status === 404) {
        console.error('Image doesn\'t exist', error);
      }
      else if (error.status === 401) {
        this.router.navigate(['/password/image/' + imageIdFromRoute]);
      }
      else {
        console.error('An error occurred:', error);
      }
    });

    const headers = new HttpHeaders().set('API-SESSION-KEY', sessionKey || '');
    // Make the HTTP request to get image data
    this.http.get<any>('https://localhost:7063/api/Public/GetImageWithSourceFile/' + imageIdFromRoute, { headers }).subscribe(p => {
      this.contentType = p.content.contentType;
      this.data = p.content.data;
    });
  }
}
