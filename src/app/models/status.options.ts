import { Status } from "./status.enum";

export const STATUS_OPTIONS = [
  { display: 'Aberto', value: Status.Open },
  { display: 'Em Progresso', value: Status.InProgress },
  { display: 'Resolvido', value: Status.Resolved },
  { display: 'Fechado', value: Status.Closed }
];
