import { Component, inject, Input } from '@angular/core';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { IDepartment } from '../../../types/department';
import { HttpService } from '../../../services/http.service';
import { IEmployee } from '../../../types/employee';
import {
  MAT_DIALOG_DATA,
  MatDialogConfig,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';

@Component({
  selector: 'app-employee-form',
  imports: [
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatSelectModule,
    MatButtonModule,
    MatRadioModule,
    MatDatepickerModule,
    MatIconModule,
    CommonModule,
    MatDialogModule,
  ],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.scss',
})
export class EmployeeFormComponent {
  fb = inject(FormBuilder);
  @Input() employeeId!: number;
  employeeForm = this.fb.group({
    id: [0],
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
    gender: [1, Validators.required],
    salary:[],
    departmentId: ['', Validators.required],
    jobTitle: ['', [Validators.required]],
    joiningDate: [null, [Validators.required]],
    lastWorkingDate: [],
    dateOfBirth: [null, [Validators.required]],
  });

  departments: IDepartment[] = [];
  httpService = inject(HttpService);
  ngOnInit() {
    this.httpService.getDepartments({}).subscribe((result) => {
      this.departments = result.data;
    });
    console.log('here', this.data);
    if (this.data.employeeId) {
      this.httpService
        .getEmployeeById(this.data.employeeId)
        .subscribe((result) => {
          console.log(result);
          this.employeeForm.patchValue(result as any);
          this.employeeForm.get('gender')?.disable();
          this.employeeForm.get('joiningDate')?.disable();
          this.employeeForm.get('dateOfBirth')?.disable();
        });
    } else {
    }
  }
  dialogRef = inject(MatDialogRef<EmployeeFormComponent>);
  data = inject<EmployeeFormComponent>(MAT_DIALOG_DATA);
  onSubmit() {
    if (this.data.employeeId) {
      let value: any = this.employeeForm.value;
      this.httpService
        .updateEmployee(this.data.employeeId, value)
        .subscribe(() => {
          alert('Record updated');
          this.dialogRef.close();
        });
    } else {
      console.log('valid', this.employeeForm.valid);
      let value: any = this.employeeForm.value;
      this.httpService.addEmployee(value).subscribe(() => {
        alert('Record saved');
        this.dialogRef.close();
      });
    }
  }
}
