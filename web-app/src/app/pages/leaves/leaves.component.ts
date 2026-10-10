import { Component, inject, OnInit } from '@angular/core';
import { LeaveService } from '../../services/leave.service';
import { PagedData } from '../../types/paged-data';
import { ILeave, LeaveStatus, LeaveType } from '../../types/leave';
import { TableComponent } from '../../components/table/table.component';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-leaves',
  imports: [TableComponent],
  templateUrl: './leaves.component.html',
  styleUrls: ['./leaves.component.scss'],
})
export class LeavesComponent implements OnInit {
  leaveService = inject(LeaveService);
  authService = inject(AuthService);

  filter = {
    pageIndex: 0,
    pageSize: 5,
  };

  data!: PagedData<ILeave>;

  ngOnInit() {
    this.getLeavesData();
  }

  // ================= TABLE COLUMNS =================
  showCols = [
    'id',
    {
      key: 'type',
      format: (rowData: ILeave) => {
        switch (rowData.type) {
          case LeaveType.Casual:
            return 'Casual Leave';
          case LeaveType.Sick:
            return 'Sick Leave';
          case LeaveType.Earned:
            return 'Earned Leave';
          default:
            return '-';
        }
      },
    },
    'reason',
    {
      key: 'leaveDate',
      // ✅ FIX: Explicit date formatting (DateOnly-safe)
      format: (rowData: ILeave) => {
        if (!rowData.leaveDate) return '-';

        // leaveDate comes as yyyy-MM-dd
        const date = new Date(rowData.leaveDate + 'T00:00:00');
        return date.toLocaleDateString('en-GB', {
          day: '2-digit',
          month: 'short',
          year: 'numeric',
        });
      },
    },
    {
      key: 'status',
      format: (rowData: ILeave) => {
        switch (rowData.status) {
          case LeaveStatus.Pending:
            return 'Pending';
          case LeaveStatus.Accepted:
            return 'Accepted';
          case LeaveStatus.Rejected:
            return 'Rejected';
          case LeaveStatus.Canelled:
            return 'Cancelled';
          default:
            return '-';
        }
      },
    },
    {
      key: 'action',
      format: (rowData: ILeave) => {
        if (this.authService.isEmployee) {
          if (rowData.status === LeaveStatus.Pending) {
            return ['Cancel'];
          }
        } else {
          if (rowData.status === LeaveStatus.Pending) {
            return ['Reject', 'Accept'];
          }
        }
        return [];
      },
    },
  ];

  // ================= LOAD DATA =================
  getLeavesData() {
    this.leaveService.getLeaves(this.filter).subscribe((result) => {
      this.data = result;
    });
  }

  pageChange(event: any) {
    this.filter.pageIndex = event.pageIndex;
    this.getLeavesData();
  }

  // ================= ACTION HANDLER =================
  onRowClick(event: any) {
    switch (event.btn) {
      case 'Cancel':
        this.leaveService
          .updateLeaveStatus(event.rowData.id, LeaveStatus.Canelled)
          .subscribe(() => this.getLeavesData());
        break;

      case 'Accept':
        this.leaveService
          .updateLeaveStatus(event.rowData.id, LeaveStatus.Accepted)
          .subscribe(() => this.getLeavesData());
        break;

      case 'Reject':
        this.leaveService
          .updateLeaveStatus(event.rowData.id, LeaveStatus.Rejected)
          .subscribe(() => this.getLeavesData());
        break;
    }
  }
}
