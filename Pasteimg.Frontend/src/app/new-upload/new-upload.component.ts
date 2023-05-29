import { Component, ElementRef, ViewChild, Inject, PLATFORM_ID } from '@angular/core';
import {Upload, UploadService, Image, Content} from "../upload.service";
import { trigger, state, style, animate, transition } from '@angular/animations';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { MatDialog, MatDialogConfig, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {FormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import { Clipboard } from '@angular/cdk/clipboard';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';

interface UploadedFile {
  name: string;
  description: string;
  file: File;
  NSFW: boolean;
  password: string;
  url: string;
}

@Component({
  selector: 'app-new-upload',
  templateUrl: './new-upload.component.html',
  styleUrls: ['./new-upload.component.scss'],
  animations: [
    trigger('removeAnimation', [
      state('true', style({ opacity: 0, transform: 'scale(0.5)' })),
      transition('* => true', [
        animate('600ms ease-out', style({ opacity: 1, transform: 'scale(1)' }))
      ])
    ])
  ]
})

export class NewUploadComponent {
  files: UploadedFile[] = [];
  hasWrongFileExtension = false;
  showNotAllowed: boolean = false;
  fileRemovalAnimations: boolean[] = []; // Define the file removal animations array
  firstUpload: boolean = true;
  fileuploadLimit: number = 6;
  @ViewChild('sectionRef', { static: false, read: ElementRef }) sectionRef!: ElementRef;

  constructor(
    private uploadService: UploadService,
    private _snackBar: MatSnackBar,
    private router: Router,
    private dialog: MatDialog,
    @Inject(DOCUMENT) private document: Document,
    @Inject(PLATFORM_ID) private platformId: any
  ) {}

  /**
   * on file drop handler
   */
  onFileDropped($event: any | null) {
    if ($event) {
      const files = $event instanceof FileList ? Array.from($event) : [$event];
      if (files && files.length+this.files.length > this.fileuploadLimit) {
        const message = `You can only submit ${this.fileuploadLimit} images at once!\n`;
  
        this._snackBar.open(message, 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 3000
        });
        return;
      }
      else if (files) {
        const validFiles: any[] = this.filterValidFiles(Array.from(files));
        this.prepareFilesList(validFiles);
        this.hasWrongFileExtension = files.length !== validFiles.length;
  
        if (this.hasWrongFileExtension) {
          const invalidFiles: File[] = Array.from(files).filter(file => !validFiles.includes(file));
          const invalidFileExtensions: string[] = invalidFiles.reduce((extensions: string[], file: File) => {
            const filenameParts = file.name.split('.');
            const extension = filenameParts[filenameParts.length - 1]; // Get the last part as the extension
            if (!extensions.includes(extension)) {
              extensions.push(extension);
            }
            return extensions;
          }, []);
          const message = `Some of the files you tried to upload have unsupported extensions: ${invalidFileExtensions.join(', ')}`;
  
          this._snackBar.open(message, 'Close', {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 3000
          });
        }
      }
    }
    
    this.showNotAllowed = this.hasWrongFileExtension;
    setTimeout(() => {
      this.showNotAllowed = false;
    }, 2000);
  }
  

  /**
   * handle file from browsing
   */
  fileBrowseHandler(files: FileList | null) {
    if (files && files.length + this.files.length > this.fileuploadLimit) {
      const message = `You can only submit ${this.fileuploadLimit} images at once!`;
      this._snackBar.open(message, 'Close', {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 3000
      });
      return;
    } else if (files) {
      const validFiles: any[] = this.filterValidFiles(Array.from(files));
      this.prepareFilesList(validFiles);
      this.hasWrongFileExtension = files.length !== validFiles.length;
  
      if (this.hasWrongFileExtension) {
        const invalidFiles: File[] = Array.from(files).filter(file => !validFiles.includes(file));
        const invalidFileExtensions: string[] = invalidFiles.reduce((extensions: string[], file: File) => {
          const filenameParts = file.name.split('.');
          const extension = filenameParts[filenameParts.length - 1]; // Get the last part as the extension
          if (!extensions.includes(extension)) {
            extensions.push(extension);
          }
          return extensions;
        }, []);
        const message = `Some of the files you tried to upload have unsupported extensions: ${invalidFileExtensions.join(', ')}`;
      
        this._snackBar.open(message, 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 3000
        });
      }
    }
  
    this.showNotAllowed = this.hasWrongFileExtension;
    setTimeout(() => {
      this.showNotAllowed = false;
    }, 2000);
  }
  

  /**
   * Delete file from files list
   * @param index (File index)
   */
  deleteFile(index: number) {
    this.files.splice(index, 1);
        // Add the class to trigger the removal animation
        setTimeout(() => {
          this.fileRemovalAnimations[index] = true;
        }, 0);
    
        // Remove the class after the animation duration
        setTimeout(() => {
          this.fileRemovalAnimations.splice(index, 1);
        }, 600);
  }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<File>) {
    const fileCount = files.length;
    let processedCount = 0;
  
    const loadFile = (file: File) => {
      const reader = new FileReader();
      reader.onload = (event: any) => {
        const uploadedFile: UploadedFile = {
          file: file,
          name: file.name,
          url: event.target.result,
          description: '',
          NSFW: false,
          password: '',
        };
        this.files.push(uploadedFile);
      };
      reader.readAsDataURL(file);
    };
  
    files.forEach((file) => loadFile(file));
    this.scrollToSectionEnd();
  }
  

  /**
   * format bytes
   * @param bytes (File size in bytes)
   * @param decimals (Decimals point)
   */
  formatBytes(bytes: number, decimals: number) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }

  private filterValidFiles(files: File[]): File[] {
    const allowedExtensions = ['.jpg', '.jpeg', '.jpe', '.jif', '.jfif', '.jfi', '.gif', '.png', '.apng', '.webp', '.bmp'];
    
    return files.filter(file => {
      const fileExtension = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));
      return allowedExtensions.includes(fileExtension);
    });
  }
  
  async submitUpload() {
    const images: Image[] = [];

    for (const file of this.files) {
      let data=await this.readFileAsByteArray(file.file);
      let content: Content=new Content(file.file.type, data, file.name);
      /* const content = await this.readFileAsByteArray(file.file); */
      const image: Image = {
        content: content,
        description: file.description,
        id: '',
        nsfw: file.NSFW,
        uploadId: ''
      };
      images.push(image);
    }

    const upload: Upload = {
      images: images,
      id: '',
      password: '',
      timeStamp: new Date().toISOString()
    };

    // Call the upload service to submit the upload
    const uploadedUpload = await this.uploadService.postUpload(upload);
    console.log("alma");
    this.files=[];
    let message = `Files uploaded!`;
    if (uploadedUpload==""){
      message="Files couldn't be uploaded!"
    }
    else{

    }      
    this._snackBar.open(message, 'Close', {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      duration: 3000
    });
    //this.router.navigate(['link/'+uploadedUpload]);
    this.openDialog(uploadedUpload);
    
    // Handle the response as needed
    // ...
  }


  private async readFileAsByteArray(file: File): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      const chunkSize = 1024 * 1024; // 1MB chunk size
      let offset = 0;
      let base64String = '';
  
      const readChunk = () => {
        const blob = file.slice(offset, offset + chunkSize);
        reader.onload = () => {
          const chunkBase64 = reader.result as string;
          base64String += chunkBase64.split(',')[1];
          offset += chunkSize;
  
          if (offset < file.size) {
            readChunk();
          } else {
            resolve(base64String);
          }
        };
        reader.onerror = () => {
          reject(reader.error);
        };
        reader.readAsDataURL(blob);
      };
  
      readChunk();
    });
  }
  scrollToSectionEnd() {
    if (this.sectionRef && this.sectionRef.nativeElement) {
      const sectionElement = this.sectionRef.nativeElement as HTMLElement;
      sectionElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
    }
  }
 
  openDialog(idOfThheUpload:string) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.data={ url: (this.getBaseUrl()+"/link/"+idOfThheUpload) };

    this.dialog.open(DialogContentComponent, dialogConfig);
  }
  private getBaseUrl(): string {
    if (isPlatformBrowser(this.platformId)) {
      return this.document.location.origin + '/';
    } else {
      // Return a fallback value if not running in a browser environment
      return '';
    }
  }

}

@Component({
  selector: 'app-dialog-content',
  templateUrl: 'modalHTML.html',
  standalone: true,
  imports: [MatDialogModule, MatIconModule, MatButtonModule, FormsModule, MatInputModule]
})
export class DialogContentComponent {
  constructor(
    private dialogRef: MatDialogRef<DialogContentComponent>,
    private clipboard: Clipboard,
    @Inject(DOCUMENT) private document: Document,
    @Inject(PLATFORM_ID) private platformId: any,
    @Inject(MAT_DIALOG_DATA) public data: { url: string },
    private _snackBar: MatSnackBar,
  ) {}

  copyToClipboard() {
    this.clipboard.copy(this.data.url);
    const message="Copied!"
    this._snackBar.open(message, 'Close', {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      duration: 2000
    });
  }

  private getBaseUrl(): string {
    if (isPlatformBrowser(this.platformId)) {
      return this.document.location.origin + '/';
    } else {
      // Return a fallback value if not running in a browser environment
      return '';
    }
  }

  selectInputText(event: MouseEvent) {
    const inputElement = event.target as HTMLInputElement;
    inputElement.select();
  }
  // ... Implement the dialog component functionality ...
}
