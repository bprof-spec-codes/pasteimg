export class Content {

  public contentType: string = ''
  //public data: Uint8Array = []
  public fileName: string = ''

}

export class Image {
  public content: Content = new Content()
  public description: string = ''
  public id: string = ''
  public nsfw: boolean = false
  public upload: Upload = new Upload()
  public uploadId: string = ''
}

export class Upload {
  public id: string = ''
  public images: Array<Image> = new Array<Image>()
  public uploadDate: Date = new Date()

}
