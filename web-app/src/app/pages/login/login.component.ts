import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  authService = inject(AuthService);
  fb = inject(FormBuilder);
  router = inject(Router);

  loginForm!: FormGroup;

  // 👁️ Password toggle (NEW – SAFE)
  hidePassword = true;

  ngOnInit() {
    this.loginForm = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });

    if (this.authService.isLoggedIn) {
      this.router.navigateByUrl('/');
    }
  }

  onLogin() {
    this.authService
      .login(
        this.loginForm.value.email,
        this.loginForm.value.password
      )
      .subscribe((result) => {
        this.authService.saveToken(result);

        if (result.role === 'Admin') {
          this.router.navigateByUrl('/');
        } else {
          this.router.navigateByUrl('/employee-dashboard');
        }
      });
  }
}
