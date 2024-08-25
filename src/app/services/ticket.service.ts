import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

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
  private apiUrl: string = `${environment.apiUrl}/tickets`

  constructor(
    private http: HttpClient
  ) { }

  create(ticket: TicketRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, ticket);
  }

  findAll(page: number, size: number, filter?: any): Observable<{ items: any[], total: number }> {
    let params = new HttpParams()
      .set('page', page)
      .set('size', size);

    if (filter) {
      if (filter.status) {
        params = params.set('status', filter.status);
      }
      if (filter.priority) {
        params = params.set('priority', filter.priority);
      }
      if (filter.type) {
        params = params.set('type', filter.type);
      }
      if (filter.createdAfter) {
        params = params.set('createdAfter', filter.createdAfter);
      }
      if (filter.createdBefore) {
        params = params.set('createdBefore', filter.createdBefore);
      }
      if (filter.subject) {
        params = params.set('subject', filter.subject);
      }
    }

    return this.http.get<{ items: any[], total: number }>(this.apiUrl, { params });
  }

  findOne(id: string | number): Observable<any> {
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
