import { Component } from '@angular/core';
import {Image, Upload} from "./model";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Pasteimg.Frontend';

  uploads: Array<Upload> = [];
  images:  Array<Image> = [];

}
