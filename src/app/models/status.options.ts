import { Status } from "./status.enum";

export const STATUS_OPTIONS = [
  { label: 'Aberto', value: Status.Open },
  { label: 'Em Progresso', value: Status.InProgress },
  { label: 'Resolvido', value: Status.Resolved },
  { label: 'Fechado', value: Status.Closed }
];
