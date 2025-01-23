import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http : HttpClient) { }

  private userUrl = 'https://localhost:7014/api/User'

  addUser(formData:any):Observable<any>{
    return this.http.post<any>(`${this.userUrl}/add-user`,formData)
  }

  loginUser(loginDetails:any):Observable<any>{
    return this.http.post<any>(`${this.userUrl}/user-login`,loginDetails)
  }

  getRoles():Observable<any>{
    return this.http.get<any>(`${this.userUrl}/get-roles`)
  }

}
