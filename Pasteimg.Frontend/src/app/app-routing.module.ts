import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ViewUploadComponent} from "./view-upload/view-upload.component";
import {HomeComponent} from "./home/home.component";

const routes: Routes = [
  {path: "", component: HomeComponent},
  {path: "link/:uploadId", component: ViewUploadComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
