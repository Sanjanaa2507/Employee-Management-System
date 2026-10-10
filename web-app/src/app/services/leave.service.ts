import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { PagedData } from '../types/paged-data';
import { ILeave } from '../types/leave';
import { IAttendace } from '../types/attendace';

@Injectable({
  providedIn: 'root',
})
export class LeaveService {
  private http = inject(HttpClient);

  private jsonHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
  });

  // ================= APPLY LEAVE =================
  applyLeave(type: number, reason: string, date: string | Date) {
    // ✅ FIX: always send yyyy-MM-dd (DateOnly-safe)
    const leaveDate =
      date instanceof Date
        ? date.toISOString().split('T')[0]
        : date;

    return this.http.post(
      `${environment.apiUrl}/api/Leave/apply`,
      {
        type,
        reason,
        leaveDate: leaveDate, // yyyy-MM-dd
      },
      { headers: this.jsonHeaders }
    );
  }

  // ================= GET LEAVES =================
  getLeaves(filter: any) {
    const params = new HttpParams({ fromObject: filter });

    return this.http.get<PagedData<ILeave>>(
      `${environment.apiUrl}/api/Leave`,
      { params }
    );
  }

  // ================= ACCEPT / REJECT =================
  updateLeaveStatus(id: number, status: number) {
    return this.http.post(
      `${environment.apiUrl}/api/Leave/update-status`,
      {
        id,
        status, // 1 = Accepted, 2 = Rejected
      },
      { headers: this.jsonHeaders }
    );
  }

  // ================= ATTENDANCE =================
  markPresent() {
    return this.http.post(
      `${environment.apiUrl}/api/Attendance/mark-present`,
      {},
      { headers: this.jsonHeaders }
    );
  }

  getAttendaceHistory(filter: any) {
    const params = new HttpParams({ fromObject: filter });

    return this.http.get<PagedData<IAttendace>>(
      `${environment.apiUrl}/api/Attendance`,
      { params }
    );
  }
}
