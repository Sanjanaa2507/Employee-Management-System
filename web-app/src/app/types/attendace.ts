export interface IAttendace {
  type: AttendaceType;
  date: string;
}

export enum AttendaceType {
  Present = 1,
  Leave = 2,
}
