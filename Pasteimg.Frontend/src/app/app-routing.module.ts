import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ViewUploadComponent} from "./view-upload/view-upload.component";
import { NewUploadComponent } from './new-upload/new-upload.component';
import {ViewImageComponent} from "./view-image/view-image.component";
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import {ViewAdminComponent} from "./view-admin/view-admin.component";

const routes: Routes = [
  /* {path: "", component: HomeComponent}, */
  {path: "link/:uploadId", component: ViewUploadComponent},
  {path: "link/image/:imageId", component: ViewImageComponent},
  {path: "upload", component: NewUploadComponent},
  {path: "register", component: RegisterComponent},
  {path: "", component: NewUploadComponent},
  {path: "login", component: LoginComponent},
  {path: "admin", component: ViewAdminComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
