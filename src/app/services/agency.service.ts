import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AgencyService {
  private apiUrl: string = 'http://localhost:3000/api/agencies';

  constructor(
    private http: HttpClient
  ) { }

  findAll(page: number, size: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}?page=${page}&size=${size}`, { withCredentials: true });
  }
}
