import { Component, inject, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { DashboardService } from '../../services/dashboard.service';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { IDepartmentData } from '../../types/dahsboard';
import { TableComponent } from '../../components/table/table.component';
import { PagedData } from '../../types/paged-data';
import { ILeave, LeaveStatus, LeaveType } from '../../types/leave';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-home',
  imports: [
    MatCardModule,
    BaseChartDirective,
    TableComponent,
    FormsModule,
    MatSelectModule,
    MatFormFieldModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {

  /* 🔥 REQUIRED FOR CHART REFRESH */
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  /* 🔥 REQUIRED BY TEMPLATE (FIXES YOUR ERRORS) */
  public barChartLegend = true;
  public barChartPlugins = [];

  /* ================= DASHBOARD COUNTS ================= */

  salaryForMonth!: number;
  employeeCount!: number;
  departmentCount!: number;

  dashboardService = inject(DashboardService);

  /* ================= MONTH & YEAR ================= */

  currentYear = new Date().getFullYear();
  currentMonth = new Date().getMonth() + 1;

  months = [
    { value: 1, name: 'January' },
    { value: 2, name: 'February' },
    { value: 3, name: 'March' },
    { value: 4, name: 'April' },
    { value: 5, name: 'May' },
    { value: 6, name: 'June' },
    { value: 7, name: 'July' },
    { value: 8, name: 'August' },
    { value: 9, name: 'September' },
    { value: 10, name: 'October' },
    { value: 11, name: 'November' },
    { value: 12, name: 'December' },
  ];

  years = [2025, 2024, 2023];

  /* ================= ABSENT SUMMARY ================= */

  absentSummary: any[] = [];

  /* ================= DEPARTMENT CHART ================= */

  departmentData!: IDepartmentData[];

  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [{ label: 'Department Count', data: [] }],
  };

  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
  };

  /* ================= ABSENT SUMMARY CHART ================= */

  public absentChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [{ label: 'Absent Days', data: [], backgroundColor: [] }],
  };

  public absentChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
  };

  /* ================= INIT ================= */

  ngOnInit() {

    this.dashboardService.getDashboardData().subscribe(res => {
      this.salaryForMonth = res.totalSalary;
      this.employeeCount = res.employeeCount;
      this.departmentCount = res.departmentCount;
    });

    this.dashboardService.getDepartmentData().subscribe(res => {
      this.barChartData = {
        labels: res.map(x => x.name),
        datasets: [{ label: 'Department Count', data: res.map(x => x.employeeCount) }],
      };
      this.departmentData = res;
    });

    this.dashboardService.getTodayLeaveData().subscribe(res => {
      this.leaveData = {
        data: res,
        totalData: res.length,
      };
    });

    this.loadAbsentSummary();
  }

  /* ================= LOAD ABSENT SUMMARY ================= */

  loadAbsentSummary() {
    this.dashboardService
      .getAbsentSummary(this.currentYear, this.currentMonth)
      .subscribe(res => {

        this.absentSummary = res ?? [];

        // Reset chart
        this.absentChartData = {
          labels: [],
          datasets: [{ label: 'Absent Days', data: [] }],
        };

        if (!res || res.length === 0) {
          setTimeout(() => this.chart?.update());
          return;
        }

        this.absentChartData = {
          labels: res.map(x => x.employeeName),
          datasets: [
            {
              label: 'Absent Days',
              data: res.map(x => x.absentDays),
              backgroundColor: res.map(
                (_, i) =>
                  ['#ef4444', '#3b82f6', '#22c55e', '#f59e0b', '#8b5cf6'][i % 5]
              ),
            },
          ],
        };

        setTimeout(() => this.chart?.update());
      });
  }

  /* ================= LEAVE TABLE ================= */

  leaveCols = [
    'employeeName',
    {
      key: 'type',
      format: (row: ILeave) => {
        switch (row.type) {
          case LeaveType.Casual: return 'Casual leave';
          case LeaveType.Sick: return 'Sick leave';
          case LeaveType.Earned: return 'Earned leave';
        }
      },
    },
    {
      key: 'status',
      format: (row: ILeave) => {
        switch (row.status) {
          case LeaveStatus.Pending: return 'Pending';
          case LeaveStatus.Rejected: return 'Rejected';
          case LeaveStatus.Accepted: return 'Accepted';
          case LeaveStatus.Canelled: return 'Cancelled';
        }
      },
    },
    'reason',
  ];

  leaveData!: PagedData<ILeave>;
}