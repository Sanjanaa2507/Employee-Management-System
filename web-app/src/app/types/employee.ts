export interface IEmployee {
  id: number;
  name: string;
  email: string;
  phone: string;
  jobTitle: string;
  gender: Gender;
  departmentId: number;
  joiningDate: string;
  lastWorkingDate: string;
  dateOfBirth: string;
  salary:number;
}

export enum Gender {
  Male = 1,
  Female = 2,
}
