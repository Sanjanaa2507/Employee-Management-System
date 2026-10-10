import { Component, inject } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { TableComponent } from '../../components/table/table.component';
import { IEmployee } from '../../types/employee';
import { MatButtonModule } from '@angular/material/button';
import {
  MatDialog,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { EmployeeFormComponent } from './employee-form/employee-form.component';
import { MatInputModule } from '@angular/material/input';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { debounceTime } from 'rxjs';
import { PagedData } from '../../types/paged-data';
import { Router } from '@angular/router';

@Component({
  selector: 'app-employee',
  imports: [
    TableComponent,
    MatButtonModule,
    MatInputModule,
    ReactiveFormsModule,
    MatFormFieldModule,
  ],
  templateUrl: './employee.component.html',
  styleUrl: './employee.component.scss',
})
export class EmployeeComponent {
  httpService = inject(HttpService);
  pagedEmployeeData!: PagedData<IEmployee>;

  showCols = ['id', 'name', 'email', 'phone', {
    key:'action',
    format:()=>{
      return ["Edit","Delete","Attendace"]
    }
  }];
  filter: any = {
    pageIndex: 0,
    pageSize: 2,
  };

  ngOnInit() {
    this.getLatestDate();
    this.searchControl.valueChanges
      .pipe(debounceTime(300))
      .subscribe((result: string | null) => {
        console.log(result);
        this.filter.search = result;
        this.filter.pageIndex = 0;
        this.getLatestDate();
      });
  }
  searchControl = new FormControl('');
  totalData!: number;
  getLatestDate() {
    this.httpService.getEmployeeList(this.filter).subscribe((result) => {
      this.pagedEmployeeData = result;
    });
  }
  edit(employee: IEmployee) {
    let ref = this.dialog.open(EmployeeFormComponent, {
      panelClass: 'm-auto',
      data: {
        employeeId: employee.id,
      },
    });
    ref.afterClosed().subscribe((result) => {
      this.getLatestDate();
    });
  }
  delete(employee: IEmployee) {
    console.log(employee);
    this.httpService.deleteEmployee(employee.id).subscribe(() => {
      alert('Record Deleted.');
      this.getLatestDate();
    });
  }
  add() {
    this.openDialog();
  }
  readonly dialog = inject(MatDialog);
  openDialog(): void {
    let ref = this.dialog.open(EmployeeFormComponent, {
      panelClass: 'm-auto',
      data: {},
    });
    ref.afterClosed().subscribe((result) => {
      this.getLatestDate();
    });
  }

  pageChange(event: any) {
    console.log(event);
    this.filter.pageIndex = event.pageIndex;
    this.getLatestDate();
  }
  router=inject(Router);
  onRowClick(event:any){
     if(event.btn==="Edit"){
      this.edit(event.rowData)
     }
     if(event.btn==="Delete"){
      this.delete(event.rowData)
     }
     if(event.btn==="Attendace"){
       this.router.navigateByUrl("/attendace/"+event.rowData.id);
     }
  }
}
