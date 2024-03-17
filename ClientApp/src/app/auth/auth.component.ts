import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountsService, Configuration } from '../../../build'
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {

  constructor(private formBuilder: FormBuilder, private service: AccountsService, private snackBar: MatSnackBar, private router: Router) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  loginForm: FormGroup;
  loading = false;

  get f(): any { return this.loginForm.controls; }

  onSubmit() {
    // Check if the form is valid
    if (this.loginForm.invalid) {
      return;
    }
    this.loginForm.disable();
    this.loading = true;
    // Proceed with form submission
    this.service.apiAccountsSignInPost(this.loginForm.value).subscribe(data => {
      localStorage.setItem('accessToken', data.accessToken ?? '');
      localStorage.setItem('userName', data.userName ?? '');
      this.snackBar.open(`Welcome ${data.userName} your sign in was successful`, undefined, {duration: 2000, verticalPosition: 'top'});
      this.router.navigate(['/dashboard']);
    }, (err: HttpErrorResponse) => {
      this.loading = false;
      this.loginForm.enable();
      this.snackBar.open(`API (${err.status}) Error: ${err.error}`, undefined, {duration: 2000, verticalPosition: 'top'});
    });
  }
}
