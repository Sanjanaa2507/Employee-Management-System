import { Component, inject } from '@angular/core';
import { TableComponent } from '../../components/table/table.component';
import { LeaveService } from '../../services/leave.service';
import { PagedData } from '../../types/paged-data';
import { AttendaceType, IAttendace } from '../../types/attendace';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-attendace',
  imports: [TableComponent],
  templateUrl: './attendace.component.html',
  styleUrl: './attendace.component.scss',
})
export class AttendaceComponent {
  filter = {
    pageIndex: 0,
    pageSize: 5,
    employeeId:''
  };
  leaveService = inject(LeaveService);
  employeeId!: string | null;
  showCols: any[] = [
    {
      key: 'date',
      format: (rowData: IAttendace) => {
        let date = new Date(rowData.date);
        return (
          date.getDate() +
          '/' +
          (date.getMonth() + 1) +
          '/' +
          date.getFullYear()
        );
      },
    },
    {
      key: 'type',
      format: (rowData: IAttendace) => {
        switch (rowData.type) {
          case AttendaceType.Leave:
            return 'Leave';
          case AttendaceType.Present:
            return 'Present';
        }
      },
    },
  ];
  route = inject(ActivatedRoute);
  ngOnInit() {
    this.employeeId = this.route.snapshot.paramMap.get('id');
    this.getLatestData();
  }

  data!: PagedData<IAttendace>;
  getLatestData() {
    if(this.employeeId){
      this.filter.employeeId = this.employeeId as string;
    }
    this.leaveService.getAttendaceHistory(this.filter).subscribe((result) => {
      this.data = result;
    });
  }
  pageChange(event: any) {
    console.log(event);
    this.filter.pageIndex = event.pageIndex;
    this.getLatestData();
  }
}
