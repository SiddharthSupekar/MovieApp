import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  constructor(private http : HttpClient) { }

  private movieUrl = 'https://localhost:7014/api/Movie'

  getMovies():Observable<any>{
    return this.http.get<any>(`${this.movieUrl}/get-all-movies`)
  }
  getSearchMovies(searchTitle:string,apiKey:string):Observable<any>{
    return this.http.get<any>(`${this.movieUrl}/get-searched-movies?movieTitle=${searchTitle}&apiKey=${apiKey}`)
  }
  addMovie(details:any):Observable<any>{
    return this.http.post<any>(`${this.movieUrl}/add-movie`, details)
  }
  updateMovie(movieId:number, updateDetails:any ):Observable<any>{
    return this.http.put<any>(`${this.movieUrl}/update-movie?movieId=${movieId}`,updateDetails)
  }
  deleteMovie(movieId:number):Observable<any>{
    return this.http.delete<any>(`${this.movieUrl}/delete-movie?movieId=${movieId}`)
  }

}
