import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { DepartmentsComponent } from './pages/departments/departments.component';
import { EmployeeComponent } from './pages/employee/employee.component';
import { LoginComponent } from './pages/login/login.component';
import { EmployeedashboardComponent } from './pages/employeedashboard/employeedashboard.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { LeavesComponent } from './pages/leaves/leaves.component';
import { AttendaceComponent } from './pages/attendace/attendace.component';
import { AdminMapComponent } from './pages/admin-map/admin-map.component';

export const routes: Routes = [

  // ✅ Default redirect
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },

  // ✅ Admin dashboard
  {
    path: 'dashboard',
    component: HomeComponent,
  },

  // ✅ Employee dashboard
  {
    path: 'employee-dashboard',
    component: EmployeedashboardComponent,
  },

  {
    path: 'departments',
    component: DepartmentsComponent,
  },

  {
    path: 'employee',
    component: EmployeeComponent,
  },

  {
    path: 'login',
    component: LoginComponent,
  },

  {
    path: 'profile',
    component: ProfileComponent,
  },

  {
    path: 'leaves',
    component: LeavesComponent,
  },

  // ✅ FIXED spelling
  {
    path: 'attendace',
    component: AttendaceComponent,
  },

  {
    path: 'attendace/:id',
    component: AttendaceComponent,
  },

  // ✅ GeoFence map (ADMIN)
  {
    path: 'admin-map',
    component: AdminMapComponent,
  },

  // ✅ Fallback (VERY IMPORTANT)
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
