import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { TicketService } from "../services/ticket.service";

@Injectable({ providedIn: 'root' })
export class TicketResolver implements Resolve<any> {
  constructor(private ticketService: TicketService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {
    return this.ticketService.findOne(route.paramMap.get('id')!);
  }
}