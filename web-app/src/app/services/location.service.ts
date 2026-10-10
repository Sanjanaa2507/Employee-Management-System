import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class LocationService {
  private http = inject(HttpClient);

  // ===================== AUTH HEADER =====================
  private getAuthHeaders(): HttpHeaders {
    const auth = localStorage.getItem('auth');
    const token = auth ? JSON.parse(auth).token : null;

    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });
  }

  // ===================== EMPLOYEE CHECK-IN =====================
  sendLocation(latitude: number, longitude: number) {
    return this.http.post(
      `${environment.apiUrl}/api/location/check`,
      { latitude, longitude },
      {
        headers: this.getAuthHeaders(),
      }
    );
  }

  // ===================== ADMIN: GET ALL EMPLOYEES =====================
  getAllEmployeeLocations() {
    return this.http.get<any[]>(
      `${environment.apiUrl}/api/location/all`,
      {
        headers: this.getAuthHeaders(),
      }
    );
  }
}
