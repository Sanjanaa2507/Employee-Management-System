import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { LeaveService } from '../../services/leave.service';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-apply-leave',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    CommonModule,
    MatIconModule,
  ],
  templateUrl: './apply-leave.component.html',
  styleUrl: './apply-leave.component.scss',
})
export class ApplyLeaveComponent {

  private fb = inject(FormBuilder);
  private leaveService = inject(LeaveService);
  private dialogRef = inject(MatDialogRef<ApplyLeaveComponent>);

  // ✅ SIMPLE + SAFE FORM (NO GENERIC TYPING)
  leaveForm = this.fb.group({
    type: [null, Validators.required],
    leaveDate: [null, Validators.required],
    reason: ['', Validators.required],
  });

  onSubmit() {
    if (this.leaveForm.invalid) {
      alert('Please fill all required fields');
      return;
    }

    const type = this.leaveForm.value.type!;
    const reason = this.leaveForm.value.reason!;
    const leaveDate = this.leaveForm.value.leaveDate as Date | null;

    if (!leaveDate) {
      alert('Leave date is required');
      return;
    }

    // ✅ LOCAL DATE (NO TIMEZONE SHIFT)
    const yyyy = leaveDate.getFullYear();
    const mm = String(leaveDate.getMonth() + 1).padStart(2, '0');
    const dd = String(leaveDate.getDate()).padStart(2, '0');
    const formattedDate = `${yyyy}-${mm}-${dd}`;

    this.leaveService
      .applyLeave(type, reason, formattedDate)
      .subscribe({
        next: () => {
          alert('Leave applied successfully');
          this.dialogRef.close();
        },
        error: () => {
          alert('Error applying leave');
        },
      });
  }
}
