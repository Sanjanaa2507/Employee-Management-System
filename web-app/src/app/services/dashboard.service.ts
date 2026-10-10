import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IDashboard, IDepartmentData } from '../types/dahsboard';
import { ILeave } from '../types/leave';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  http = inject(HttpClient);
  constructor() {}

  getDashboardData() {
    return this.http.get<IDashboard>(environment.apiUrl + '/api/Dashboard');
  }

  getDepartmentData() {
    return this.http.get<IDepartmentData[]>(
      environment.apiUrl + '/api/Dashboard/department-data'
    );
  }

  getTodayLeaveData() {
    return this.http.get<ILeave[]>(
      environment.apiUrl + '/api/Dashboard/employee-leave-today'
    );
  }
  getAbsentSummary(year: number, month: number) {
  return this.http.get<any[]>(
    environment.apiUrl + '/api/Dashboard/absent-summary',
    {
      params: {
        year: year,
        month: month
      }
    }
  );
}

}