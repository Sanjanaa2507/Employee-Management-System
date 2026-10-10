import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  imports: [
    MatCardModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent {
  authService = inject(AuthService);
  profileForm!: FormGroup;
  fb = inject(FormBuilder);
  ngOnInit() {
    this.profileForm = this.fb.group({
      email: [],
      profileImage: [],
      phone: [],
      name: [],
      password: [],
      salary:[]
    });
    this.authService.getProfile().subscribe((result: any) => {
      console.log(result);
      this.profileForm.patchValue(result);
      this.imageSrc = result.profileImage;
    });
  }
  imageSrc!: string;
  fileUpload(event: Event) {
    var target: any = event.target;
    if (target.files && target.files[0]) {
      const file = target.files[0];
      const reader = new FileReader();
      reader.onload = () => {
        this.imageSrc = reader.result as string;
        this.profileForm.patchValue({
          profileImage: this.imageSrc,
        });
        console.log(this.imageSrc);
      };
      reader.readAsDataURL(file);
    }
  }
  onUpdate() {
    this.authService
      .updateProfile(this.profileForm.value)
      .subscribe((result) => {
        alert('Profile updated');
      });
  }
}
