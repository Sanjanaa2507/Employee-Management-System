import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService {

  private unreadCount$ = new BehaviorSubject<number>(0);
  private apiUrl = `${environment.apiUrl}/api/Notification`;

  constructor(private http: HttpClient) {}

  // 🔴 LOAD unread count (bell badge)
  loadUnreadCount() {
    this.http
      .get<number>(`${this.apiUrl}/unread-count`)
      .subscribe(count => this.unreadCount$.next(count));
  }

  // 🔔 OBSERVE unread count
  getUnreadCount() {
    return this.unreadCount$.asObservable();
  }

  // 📩 GET notifications (used when bell is clicked)
  getNotifications() {
    return this.http.get<any[]>(this.apiUrl);
  }

  // ✅ MARK ONE notification as read
  markAsRead(id: number) {
    return this.http.put(`${this.apiUrl}/mark-read/${id}`, {});
  }
}
