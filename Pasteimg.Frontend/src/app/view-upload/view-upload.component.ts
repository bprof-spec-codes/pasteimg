import { Component } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { Upload, UploadService } from "../upload.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { sessionIdService } from "../sessionId.service";

@Component({
  selector: 'app-view-upload',
  templateUrl: './view-upload.component.html',
  styleUrls: ['./view-upload.component.scss']
})
export class ViewUploadComponent {
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private uploadService: UploadService,
    private http: HttpClient,
    private sessionIdService: sessionIdService
  ) {}

  upload!: Upload;
  images: any[] = []; // Array to store the images

  async ngOnInit() {
    const sessionKey = await this.sessionIdService.getSessionId();
    const routeParams = this.route.snapshot.paramMap;
    const uploadIdFromRoute = String(routeParams.get('uploadId'));

    await this.uploadService.getUpload(uploadIdFromRoute).then(u => {
      this.upload = u;
      //console.log(u)
    })
    .catch(error => {
      if (error.status === 404) {
        console.error('Image doesn\'t exist', error);
      }
      else if (error.status === 401) {
        this.router.navigate(['/password/' + uploadIdFromRoute]);
      }
      else {
        console.error('An error occurred:', error);
      }
    });

    const headers = new HttpHeaders().set('API-SESSION-KEY', sessionKey || '');
    // Iterate through each image ID and make separate HTTP requests
    for (const image of this.upload.images) {
      const imageId = image.id;
      const imageDescription = image.description;
      const nsfw = image.nsfw;
      this.http.get<any>('https://localhost:7063/api/Public/GetImageWithThumbnailFile/' + image.id, { headers }).subscribe(p => {
        const contentType = p.content.contentType;
        const data = p.content.data;
        this.images.push({ imageId, imageDescription, nsfw, contentType , data,  }); // Push the image data to the array
      });
    }
  }
}
