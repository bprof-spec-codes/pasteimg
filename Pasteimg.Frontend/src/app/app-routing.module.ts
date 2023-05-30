import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ViewUploadComponent} from "./view-upload/view-upload.component";
import { NewUploadComponent } from './new-upload/new-upload.component';
import {ViewImageComponent} from "./view-image/view-image.component";

const routes: Routes = [
  /* {path: "", component: HomeComponent}, */
  {path: "link/:uploadId", component: ViewUploadComponent},
  {path: "link/image/:imageId", component: ViewImageComponent},
  {path: "upload", component: NewUploadComponent},
  {path: "", component: NewUploadComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
