import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ViewUploadComponent} from "./view-upload/view-upload.component";
import {HomeComponent} from "./home/home.component";
import { NewUploadComponent } from './new-upload/new-upload.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  /* {path: "", component: HomeComponent}, */
  {path: "link/:uploadId", component: ViewUploadComponent},
  {path: "upload", component: NewUploadComponent},
  {path: "login", component: LoginComponent},
  {path: "register", component: RegisterComponent},
  {path: "", component: NewUploadComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
