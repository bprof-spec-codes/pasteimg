import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Image, UploadService} from "../upload.service";

@Component({
  selector: 'app-view-image',
  templateUrl: './view-image.component.html',
  styleUrls: ['./view-image.component.scss']
})
export class ViewImageComponent {
  constructor(
    private route: ActivatedRoute,
    private uploadService: UploadService
  ) {}

  image!: Image

  ngOnInit() {
    const routeParams = this.route.snapshot.paramMap;
    const imageIdFromRoute = String(routeParams.get('imageId'));

    this.uploadService.getImage(imageIdFromRoute).then(i => {
      this.image = i
    })
      .catch(error => {
        if (error.status === 404) {
          console.error('Image doesn\'t exist', error);
        }
        else {
          console.error('An error occurred:', error);
        }
      });
  };



}
