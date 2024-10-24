import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { TicketReq } from '../models/ticket-req.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl: string = `${environment.apiUrl}/tickets`;

  constructor(
    private http: HttpClient
  ) { }

  create(ticket: TicketReq): Observable<any> {
    return this.http.post<any>(this.apiUrl, ticket);
  }

  update(id: number, ticket: TicketReq): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, ticket);
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
    }

    return this.http.get<{ items: any[], total: number }>(this.apiUrl, { params });
  }

  findOne(id: string | number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  assignCurrentUser(ticketId: number, asAssignee: boolean = true): Observable<any> {
    const params = new HttpParams().set('asAssignee', asAssignee.toString());

    return this.http.put<any>(`${this.apiUrl}/${ticketId}/assign-current-user`, {}, { params });
  }

  assignEntities(ticketId: number, entityIds: number[], entityType: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${ticketId}/${entityType}`, entityIds);
  }

  removeEntity(ticketId: number, entityId: number, entityType: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${ticketId}/${entityType}/${entityId}`);
  }
}
