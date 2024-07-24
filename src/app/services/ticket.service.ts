import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface TicketRequest {
  subject: string;
  description: string;
  type: string;
  status: string;
  priority: string;
  locationId: number;
  customerIds: number[];
  assigneeIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl: string = 'http://localhost:3000/api/tickets';

  constructor(
    private http: HttpClient
  ) { }

  create(ticket: TicketRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, ticket);
  }

  findAll(page: number, size: number): Observable<{ items: any[], total: number }> {
    return this.http.get<{ items: any[], total: number }>(`${this.apiUrl}?page=${page}&size=${size}`);
  }

  findOne(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getPriorityValues(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/priority/values`);
  }

  getStatusValues(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/status/values`);
  }

  getTypeValues(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/type/values`);
  }
}
