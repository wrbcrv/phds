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
    }

    return this.http.get<{ items: any[], total: number }>(this.apiUrl, { params });
  }

  findOne(id: string | number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addComment(ticketId: number | string, authorId: number | string, content: string): Observable<any> {
    const comment = {
      content: content
    };

    return this.http.post<any>(`${this.apiUrl}/${ticketId}/comments/${authorId}`, comment);
  }

  assignCurrentUser(ticketId: number, asAssignee: boolean = true): Observable<any> {
    const params = new HttpParams().set('asAssignee', asAssignee.toString());

    return this.http.put<any>(`${this.apiUrl}/${ticketId}/assign-current-user`, {}, { params });
  }

  assignCustomers(ticketId: number, customerIds: number[]): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${ticketId}/customers`, customerIds);
  }

  assignAssignees(ticketId: number, assigneeIds: number[]): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${ticketId}/assignees`, assigneeIds);
  }

  removeAssignee(ticketId: number, assigneeId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${ticketId}/assignees/${assigneeId}`);
  }

  removeCustomer(ticketId: number, customerId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${ticketId}/customers/${customerId}`);
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
