export class Image {
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
