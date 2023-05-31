import { HostListener, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class sessionIdService {
    backendUrl: string = 'https://localhost:7063';

    constructor() {

    }

    public async getSessionId(): Promise<string>{
        if(localStorage.getItem('sessionId') === null) await this.fetchSessionId()
        return localStorage.getItem('sessionId')!.toString()
    }

    public async fetchSessionId(){
    fetch(this.backendUrl+'/api/Public/CreateSessionKey')
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch session ID');
        }
        return response.text();
      })
      .then(sessionId => {
        localStorage.setItem('sessionId', sessionId)
      })
    }

    public clearSessionId(): void{
      localStorage.setItem('sessionId', '')
      localStorage.clear()
    }

    public setAdmimFlag(val: boolean){
      if(val) localStorage.setItem('isAdmin', 'true')
      else localStorage.setItem('isAdmin', 'false')
    }

    public isAdmin(): boolean{
      if(localStorage.getItem('isAdmin') === 'true') return true
      else return false
    }

    @HostListener('window:beforeunload', [ '$event' ])
    beforeUnloadHandler() {
      localStorage.setItem('sessionId', '')
      localStorage.clear()
  }
}