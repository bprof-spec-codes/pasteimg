import { Component } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Upload, UploadService} from "../upload.service";

@Component({
  selector: 'app-view-upload',
  templateUrl: './view-upload.component.html',
  styleUrls: ['./view-upload.component.scss']
})
export class ViewUploadComponent {
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private uploadService: UploadService
  ) {}

  upload!: Upload


  ngOnInit() {
    const routeParams = this.route.snapshot.paramMap;
    const uploadIdFromRoute = String(routeParams.get('uploadId'));

    this.uploadService.getUpload(uploadIdFromRoute).then(u => {
      this.upload = u
      //console.log(u)
     })
     .catch(error => {
      if (error.status === 404) {
        console.error('Image doesn\'t exist', error);
      }
      else if (error.status === 401) {
        this.router.navigate(['/password/'+this.upload.id]);
      }
      else{
        console.error('An error occurred:', error);
      }
    });
  };
  }
