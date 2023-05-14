import { Component } from '@angular/core';
import {Upload, UploadService, Image, Content} from "../upload.service";

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
  styleUrls: ['./new-upload.component.scss']
})
export class NewUploadComponent {
  files: UploadedFile[] = [];
  hasWrongFileExtension = false;
  showNotAllowed: boolean = false;

  constructor(
    private uploadService: UploadService
  ) {}

  /**
   * on file drop handler
   */
  onFileDropped($event: any | null) {
    if ($event) {
      const files = $event instanceof FileList ? Array.from($event) : [$event];
      if (files) {
        const validFiles: any[] = this.filterValidFiles(Array.from(files));
        this.prepareFilesList(validFiles);
        this.hasWrongFileExtension = files.length !== validFiles.length;
      }
    }
    if (this.hasWrongFileExtension) {
      this.showNotAllowed = true;
      setTimeout(() => {
        this.showNotAllowed = false;
      }, 2000);
    }
  }
  

  /**
   * handle file from browsing
   */
  fileBrowseHandler(files: FileList | null) {
    if (files) {
      const validFiles: any[] = this.filterValidFiles(Array.from(files));
      this.prepareFilesList(validFiles);
    }
  }

  /**
   * Delete file from files list
   * @param index (File index)
   */
  deleteFile(index: number) {
    this.files.splice(index, 1);
  }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<File>) {
    for (const item of files) {
      const reader = new FileReader();
      reader.onload = (event: any) => {
        const uploadedFile: UploadedFile = {
          file: item,
          name: item.name,
          url: event.target.result,
          description: '',
          NSFW: false,
          password: '',
        };
        this.files.push(uploadedFile);
      };
      reader.readAsDataURL(item);
    }
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
  
  
  
  
  
  
  
  
  

}
