import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IDepartment } from '../types/department';
import { IEmployee } from '../types/employee';
import { environment } from '../../environments/environment';
import { PagedData } from '../types/paged-data';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  http = inject(HttpClient);
  constructor() {}

  getDepartments(filter: any) {
    var params = new HttpParams({ fromObject: filter });
    return this.http.get<PagedData<IDepartment>>(
      environment.apiUrl + '/api/Department?' + params
    );
  }
  addDepartment(name: string) {
    return this.http.post(environment.apiUrl + '/api/Department', {
      name: name,
    });
  }
  updateDepartment(id: number, name: string) {
    return this.http.put(environment.apiUrl + '/api/Department/' + id, {
      name: name,
    });
  }
  deleteDepartment(id: number) {
    return this.http.delete(environment.apiUrl + '/api/Department/' + id);
  }

  getEmployeeList(filter: any) {
    var params = new HttpParams({ fromObject: filter });
    return this.http.get<PagedData<IEmployee>>(
      environment.apiUrl + '/api/Employee?' + params.toString()
    );
  }

  addEmployee(employee: IEmployee) {
    return this.http.post(environment.apiUrl + '/api/Employee', employee);
  }

  getEmployeeById(id: number) {
    return this.http.get<IEmployee>(environment.apiUrl + '/api/Employee/' + id);
  }
  updateEmployee(id: number, employee: IEmployee) {
    return this.http.put(environment.apiUrl + '/api/Employee/' + id, employee);
  }
  deleteEmployee(id: number) {
    return this.http.delete(environment.apiUrl + '/api/Employee/' + id);
  }
}
