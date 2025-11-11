import { Component, inject } from '@angular/core';
import { AuthService } from '../identity/auth.service';
import { FormGroup, FormControl, ReactiveFormsModule, Validators, ValidationErrors } from '@angular/forms'
import { Router } from '@angular/router';
import { JsonPipe } from '@angular/common';
import { ValidationService } from '../validation.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styles: ``
})
export class LoginComponent {
  router: Router = inject(Router);
  authService: AuthService = inject(AuthService);
  loginForm: FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(/^[a-z0-9_]+$/)]),
    password: new FormControl('', Validators.required)
  })

  validationService: ValidationService = inject(ValidationService);

  authenticationError: string = '';

  constructor() {
    this.validationService.setFormGroup(this.loginForm);
    this.validationService.addControl('username', [
      { name: 'required', message: "Username is required" },
      {
        name: 'pattern',
        message: "Username can only contain lowercase letters and number and underscore (_)"
      }
    ]);

    this.validationService.addControl('password', [
      { name: 'required', message: "Password is required" }
    ]);

    this.loginForm.valueChanges.subscribe(() => {
      this.authenticationError = '';
    })
  }

  login(_: any) {
    if (this.loginForm.invalid) {
      return;
    }

    const { username, password } = this.loginForm.value;
    this.authService.signIn(username, password).forEach(result => {
      if (result) {
        this.router.navigate(['/me']);
      } 
    }).catch((httpErrorResponse: HttpErrorResponse) => {
        this.authenticationError = httpErrorResponse.error;
    })

    this.loginForm.markAsTouched();
  }

  validate(controlName: string): string {
    return this.validationService.getValidationFor(controlName);
  }
}
