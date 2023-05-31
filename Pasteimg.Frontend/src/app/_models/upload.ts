export class image{
    public content: any = null
    public description: string = ''
    public id: string = ''
    public nsfw: boolean = false
    public uploadId: string = ''
}

export class Upload{
    public id: string = ''
    public password: string = ''
    public uploadId: string = ''
    public images: Array<image> = []
}