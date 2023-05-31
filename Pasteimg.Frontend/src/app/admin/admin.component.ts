import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {image, Upload} from '../_models/upload';
import { Router } from '@angular/router';
import { UploadService } from '../upload.service';
import { MatSnackBar } from '@angular/material/snack-bar';
@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})

export class AdminComponent implements OnInit {
  http: HttpClient
  header: HttpHeaders
  uploads: Array<Upload>
  searchId: string
  nav: Router
  uploadService: UploadService
  MatSnackBar: MatSnackBar

  constructor(http: HttpClient, nav: Router,  uploadService: UploadService, matSnackBar: MatSnackBar) {
   
    this.http = http
    this.uploads = []
    this.searchId = ''
    this.nav = nav
    this.uploadService= uploadService
    this.MatSnackBar = matSnackBar

    this.header = new HttpHeaders({
      'Content-Type': 'application/json',
      //'SessionKeyHeader' : localStorage.getItem('sessionId')!.toString(),
      'API-SESSION-KEY' : localStorage.getItem('sessionId')!.toString()
    })

  }
  //DONT USE - Memory Leak
  // public getThumbnail(id: string){
  //   this.http.get('https://localhost:7063/api/Admin/GetImageThumbnail/Image?id=' + id, {headers: this.header}).subscribe(p =>{
  //     console.log(p)

  //   })
  // }

  characterLimit: number= 120;
  isTextareaSelected: boolean = false;

  public find(){
    console.log(this.searchId);

  }

  public async editImage(uploadId: string, imageId:string){
    const imageToEdit = this.uploads.find(x => x.id === uploadId)?.images.find(x => x.id === imageId);
    const success= await this.uploadService.editImage(imageToEdit?.id || "", imageToEdit?.description  || "", imageToEdit?.nsfw  || false);
    let message = ``;
    if(success){
      message='Edit was successful!';
    }
    else{
      message='Edit failed!';
    }
    this.MatSnackBar.open(message, 'Close', {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      duration: 3000
    });
  }

  public async deleteImage(uploadId: string, imageId:string){
    const imageToEdit = this.uploads.find(x => x.id === uploadId)?.images.find(x => x.id === imageId);
    const success= await this.uploadService.deleteImage(imageToEdit?.id || "");
    let message = ``;
    if(success){
      const uploadIndex=this.uploads.findIndex(item => item.id === uploadId);
      const imageIndex=this.uploads[uploadIndex].images.findIndex(item => item.id === imageId);
      this.uploads[uploadIndex].images.splice(imageIndex, 1);
      if (this.uploads[uploadIndex].images.length === 0){
        this.uploads.splice(uploadIndex, 1);
      }
      message='Delete was successful!';
    }
    else{
      message='Delete failed!';
    }
    this.MatSnackBar.open(message, 'Close', {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      duration: 3000
    });
  }

  public redirectToImage(imageId: string){
    
    link/image/:imageId
  }

  ngOnInit(): void {
    this.http.get<Array<Upload>>(this.uploadService.backendUrl+'/api/Admin/GetAllUpload', {headers: this.header}).subscribe(p => {
      this.uploads = p
      console.log(this.uploads)


    })
  }

  protected readonly image = image;
}
