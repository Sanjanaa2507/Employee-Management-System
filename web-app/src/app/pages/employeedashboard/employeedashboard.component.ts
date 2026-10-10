import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ApplyLeaveComponent } from '../../components/apply-leave/apply-leave.component';
import { LeaveService } from '../../services/leave.service';
import { DashboardService } from '../../services/dashboard.service';
import { LocationService } from '../../services/location.service';

@Component({
  selector: 'app-employeedashboard',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatDialogModule],
  templateUrl: './employeedashboard.component.html',
  styleUrl: './employeedashboard.component.scss',
})
export class EmployeedashboardComponent implements OnInit {

  readonly dialog = inject(MatDialog);
  leaveService = inject(LeaveService);
  dashboardService = inject(DashboardService);
  locationService = inject(LocationService);

  isMarkingAttendance = false; // ✅ UI lock

  /* ---------------- APPLY LEAVE ---------------- */

  applyLeave() {
    this.dialog.open(ApplyLeaveComponent, {
      panelClass: 'm-auto',
      data: {},
    });
  }

  /* ---------------- ATTENDANCE (FINAL FIX) ---------------- */

  markAttendance() {
    if (this.isMarkingAttendance) return;

    this.isMarkingAttendance = true;

    if (!navigator.geolocation) {
      this.markAttendanceDirectly(); // ✅ fallback
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (position) => {
        const lat = position.coords.latitude;
        const lng = position.coords.longitude;

        // 1️⃣ Try sending location (NON-BLOCKING)
        this.locationService.sendLocation(lat, lng).subscribe({
          next: () => {
            console.log('📍 Location saved');
            this.markAttendanceDirectly();
          },
          error: () => {
            console.warn('Location failed, continuing attendance');
            this.markAttendanceDirectly(); // ✅ DO NOT BLOCK
          }
        });
      },
      () => {
        console.warn('Location denied, continuing attendance');
        this.markAttendanceDirectly(); // ✅ DO NOT BLOCK
      },
      { enableHighAccuracy: true }
    );
  }

  // ✅ SINGLE RESPONSIBILITY
  private markAttendanceDirectly() {
    this.leaveService.markPresent().subscribe({
      next: (res: any) => {
        alert(res?.message ?? 'Attendance marked successfully');
        this.isMarkingAttendance = false;
      },
      error: (err) => {
        console.error(err);
        alert(err?.error?.message ?? 'Attendance failed');
        this.isMarkingAttendance = false;
      },
    });
  }

  /* ---------------- ABSENT SUMMARY ---------------- */

  selectedYear = new Date().getFullYear();
  selectedMonth = new Date().getMonth() + 1;

  absentSummary: { employeeName: string; absentDays: number }[] = [];

  ngOnInit(): void {
    this.loadAbsentSummary();

    // background location ping
    this.sendLocation();
    setInterval(() => this.sendLocation(), 120000);
  }

  loadAbsentSummary(): void {
    this.dashboardService
      .getAbsentSummary(this.selectedYear, this.selectedMonth)
      .subscribe({
        next: (data) => (this.absentSummary = data),
        error: (err) => console.error('Absent summary error', err),
      });
  }

  /* ---------------- BACKGROUND LOCATION ---------------- */

  sendLocation(): void {
    if (!navigator.geolocation) return;

    navigator.geolocation.getCurrentPosition((position) => {
      this.locationService
        .sendLocation(position.coords.latitude, position.coords.longitude)
        .subscribe({
          next: () => console.log('📍 Background location sent'),
          error: () => {},
        });
    });
  }
}
