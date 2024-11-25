import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl: string = `${environment.apiUrl}/tickets`;

  constructor(
    private http: HttpClient
  ) { }

  getCommentsByTicketId(ticketId: number | string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${ticketId}/comments`);
  }

  addComment(ticketId: number | string, authorId: number | string, content: string, files?: File[]): Observable<any> {
    const formData: FormData = new FormData();

    if (content) {
      formData.append('content', content);
    }

    if (files && files.length > 0) {
      files.forEach((file, index) => {
        formData.append('files', file);
      });
    }

    return this.http.post<any>(`${this.apiUrl}/${ticketId}/comments/${authorId}`, formData);
  }

  updateComment(ticketId: number | string, commentId: number | string, content: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${ticketId}/comments/${commentId}`, { content });
  }

  deleteComment(ticketId: number | string, commentId: number | string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${ticketId}/comments/${commentId}`);
  }
}
