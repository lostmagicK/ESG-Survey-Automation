import { Component } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountsService } from '../../../build';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registrationForm: FormGroup;
  loading = false;

  constructor(private formBuilder: FormBuilder, private service: AccountsService, private snackBar: MatSnackBar, private router: Router) {
    this.registrationForm = this.formBuilder.group({
      fullName: ['', Validators.required],
      email: ['', {
        validators: [Validators.required, Validators.email],
        asyncValidators: [emailExistsValidator(this.service)],
        updateOn: 'blur' // Validate when the field loses focus
      }],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, {
      validator: this.passwordMatchValidator // Custom validator function to check password match
    });
  }


  get f(): any {
    return this.registrationForm.controls;
  }

  onSubmit() {
    if (this.registrationForm.invalid) {
      return;
    }
    this.loading = true;
    this.service.apiAccountsRegistrationPost(this.registrationForm.value).subscribe(data => {
      this.snackBar.open("Sign-Up successful", undefined, { duration: 2000, verticalPosition: 'top' });
      this.router.navigate(['/sign-in']);
    }, (err: HttpErrorResponse) => {
      this.loading = false;
      this.snackBar.open(`API (${err.status}) Error: ${err.error}`, undefined, { duration: 2000, verticalPosition: 'top' });
    });
  }
  passwordMatchValidator(control: any): { [key: string]: boolean } | null {
    const password = control.get('password').value;
    const confirmPassword = control.get('confirmPassword').value;
    if(password === confirmPassword){
      control.get('confirmPassword')?.setErrors(null);
      return null;
    }
    control.get('confirmPassword')?.setErrors({ 'passwordMismatch': true });
    return { 'passwordMismatch': true };
  }
}

export function emailExistsValidator(apiService: AccountsService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
    const email = control.value;
    return apiService.apiAccountsEmailExistsGet(email).pipe(
      map(res => {
        return res ? { emailExists: true } : null;
      }),
      catchError(() => of(null)) // Handle error, e.g., API request error
    );
  };
}

