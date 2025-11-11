import { Component, inject } from '@angular/core';
import { AuthService } from '../identity/auth.service';
import { FormGroup, FormControl, ReactiveFormsModule, Validators, ValidationErrors } from '@angular/forms'
import { Router } from '@angular/router';
import { ValidationService } from '../validation.service';
import { CommonModule } from '@angular/common';
import { ErrorResponse } from '../identity/dto';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent {
  router: Router = inject(Router);
  authService: AuthService = inject(AuthService);
  validationService: ValidationService = inject(ValidationService);

  registerForm: FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(/^[a-z0-9_]+$/)]),
    password: new FormControl('', Validators.required),
    confirmPassword: new FormControl('', Validators.required)
  })

  constructor() {
    this.validationService.setFormGroup(this.registerForm);
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
    this.validationService.addControl('confirmPassword', [
      { name: 'required', message: "Please confirm your password" }
    ]);
  }

  register() {
    if (this.registerForm.invalid) {
      return;
    }

    const { username, password, confirmPassword } = this.registerForm.value;

    if (password !== confirmPassword) {
      this.addCustomError('confirmPassword', 'Passwords do not match');
      return;
    }

    this.authService.register(username, password).forEach(result => {
      if (result.ok) {
        this.router.navigate(['/login']);
      }
    }).catch((httpErrorResponse: HttpErrorResponse) => {
      const response = JSON.parse(httpErrorResponse.error);
      if (Object.keys(response).includes('errors')) {
        const errorResponse: ErrorResponse = response as ErrorResponse;
        Object.keys(errorResponse.errors).forEach(key => {
          this.addCustomErrors(key.toLowerCase(), errorResponse.errors[key]);
          this.registerForm.get(key.toLowerCase())?.markAsTouched();
        })
      }
    });
  }

  private addCustomError(control: string, message: string) {
    this.registerForm.get(control)?.setErrors({ customError: true });
    this.validationService.addControl(control, [
      { name: 'customError', message: message }
    ]);
  }

  private addCustomErrors(control: string, messages: string[]) {
    this.registerForm.get(control)?.setErrors({ customError: true });
    this.validationService.addControl(control, messages.map(message => ({ name: 'customError', message: message })));
  }

  getValidations(field: string): string[] {
    return this.validationService.getValidationsFor(field);
  }

  validate(field: string): string {
    return this.validationService.getValidationFor(field);
  }
}
