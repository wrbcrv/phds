export interface TicketReq {
  subject: string;
  description: string;
  type: string;
  status: string;
  priority: string;
  locationId: number;
  customerIds: number[];
  assigneeIds: number[];
}