import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ViewUploadComponent } from './view-upload/view-upload.component';


import { HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavigationComponent } from './navigation/navigation.component';
import { FooterComponent } from './footer/footer.component';
import {MatListModule} from "@angular/material/list";
import { NewUploadComponent } from './new-upload/new-upload.component';
import { DndDirective } from './new-upload/dnd.directive';


// MATERIAL DESIGN STUFF
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatTabsModule } from "@angular/material/tabs";
import { MatTooltipModule  } from '@angular/material/tooltip';
import {MatBadgeModule} from '@angular/material/badge';
import { MatDialogModule } from '@angular/material/dialog';
import {MatExpansionModule} from '@angular/material/expansion';
// form
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { ViewImageComponent } from './view-image/view-image.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AdminComponent } from './admin/admin.component';
import { UploadPasswordCheckComponent } from './upload-password-check/upload-password-check.component';
import { ImagePasswordCheckComponent } from './image-password-check/image-password-check.component';
import {MatGridListModule} from "@angular/material/grid-list";

@NgModule({
  declarations: [
    AppComponent,
    ViewUploadComponent,
    NavigationComponent,
    FooterComponent,
    NewUploadComponent,
    DndDirective,
    ViewImageComponent,
    LoginComponent,
    RegisterComponent,
    AdminComponent,
    UploadPasswordCheckComponent,
    ImagePasswordCheckComponent,
  ],
    imports: [
        MatSlideToggleModule,
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        BrowserAnimationsModule,
        MatButtonModule,
        MatMenuModule,
        MatListModule,
        MatIconModule,
        FormsModule,
        MatFormFieldModule,
        MatCardModule,
        MatSnackBarModule,
        MatSelectModule,
        MatCheckboxModule,
        ReactiveFormsModule,
        MatInputModule,
        MatToolbarModule,
        MatSidenavModule,
        MatTabsModule,
        MatTooltipModule,
        MatBadgeModule,
        MatDialogModule,
        MatExpansionModule,
        MatGridListModule
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
