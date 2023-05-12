import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Upload, UploadService} from "../upload.service";

@Component({
  selector: 'app-view-upload',
  templateUrl: './view-upload.component.html',
  styleUrls: ['./view-upload.component.scss']
})
export class ViewUploadComponent {
  constructor(
    private route: ActivatedRoute,
    private uploadService: UploadService
  ) {}

  upload!: Upload


  ngOnInit() {
    const routeParams = this.route.snapshot.paramMap;
    const uploadIdFromRoute = String(routeParams.get('uploadId'));
    this.uploadService.getUpload(uploadIdFromRoute).then(u => {
      this.upload = u
      console.log(u)
    });
  }
}