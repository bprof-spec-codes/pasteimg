import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {image, Upload} from '../_models/upload';
import { Router } from '@angular/router';

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

  constructor(http: HttpClient, nav: Router) {
    this.http = http
    this.uploads = []
    this.searchId = ''
    this.nav = nav

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


  public find(){
    console.log(this.searchId);

  }

  public edit(id: string){
    console.log(id);

    //Ide megadni hova navigáljon ha edit van
    //this.nav.navigate([''])

  }
  public del(id: string){
    this.http.delete('https://localhost:7063/api/Admin/DeleteUpload/' + id, {headers: this.header}).subscribe(p =>{
      let index = this.uploads.findIndex(x => x.id === id)
      this.uploads.splice(index, 1)
    })
  }

  ngOnInit(): void {
    this.http.get<Array<Upload>>('https://localhost:7063/api/Admin/GetAllUpload', {headers: this.header}).subscribe(p => {
      this.uploads = p
      console.log(this.uploads)


    })
  }

  protected readonly image = image;
}
