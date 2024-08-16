import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AgencyService {
  private apiUrl: string = `${environment.apiUrl}/agencies`

  constructor(
    private http: HttpClient
  ) { }

  findAll(page: number, size: number): Observable<{ items: any[], total: number }> {
    return this.http.get<{ items: any[], total: number }>(`${this.apiUrl}`, { withCredentials: true });
  }
}
