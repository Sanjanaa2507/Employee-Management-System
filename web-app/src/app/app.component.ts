import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { AuthService } from './services/auth.service';
import { NotificationService } from './services/notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit, OnDestroy {

  authService = inject(AuthService);
  notificationService = inject(NotificationService);

  unreadCount = 0;
  notifications: any[] = [];
  showNotifications = false;

  private refreshInterval: any;

  ngOnInit() {
    // 🔔 Observe unread count
    this.notificationService
      .getUnreadCount()
      .subscribe(count => this.unreadCount = count);

    // 🔄 Initial load
    this.notificationService.loadUnreadCount();

    // 🔁 AUTO REFRESH every 1 second ✅
    this.refreshInterval = setInterval(() => {
      if (this.authService.isLoggedIn) {
        this.notificationService.loadUnreadCount();
      }
    }, 1000); // ⬅️ CHANGED FROM 5000 → 1000
  }

  // 🔔 Bell click → toggle dropdown
  onBellClick() {
    this.showNotifications = !this.showNotifications;

    if (this.showNotifications) {
      this.notificationService.getNotifications()
        .subscribe(data => {
          this.notifications = data.filter(n => !n.isRead);
        });
    }
  }

  // ✅ Mark single notification as read
  markAsRead(notification: any) {
    this.notificationService.markAsRead(notification.id)
      .subscribe(() => {
        // Remove from UI
        this.notifications =
          this.notifications.filter(n => n.id !== notification.id);

        // Refresh bell count
        this.notificationService.loadUnreadCount();
      });
  }

  ngOnDestroy() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  logout() {
    this.authService.logout();
  }
}
