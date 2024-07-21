import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  constructor() { }

  handleValidationErrors(error: HttpErrorResponse): { [key: string]: string[] } {
    const validationErrors: { [key: string]: string[] } = {};
    if (error.status === 400 && error.error && error.error.message) {
      for (const err of error.error.message) {
        const property = err.property;
        const constraints = err.constraints;
        validationErrors[property] = Object.values(constraints);
      }
    }
    return validationErrors;
  }
}
