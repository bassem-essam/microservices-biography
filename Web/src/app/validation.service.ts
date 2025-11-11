import { Injectable } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

export interface PrettyValidator {
  name: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ValidationService {
  formGroup: FormGroup = new FormGroup({});
  controls: { [name: string]: PrettyValidator[] } = {};

  constructor() { }

  setFormGroup(formGroup: FormGroup) {
    this.formGroup = formGroup;
  }

  addControl(name: string, validators: PrettyValidator[]) {
    this.controls[name] = validators;
  }

  public getValidationFor(controlName: string): string {
    const control = this.formGroup.get(controlName) as FormControl;
    if (control.untouched) return '';

    const validators = this.controls[controlName] ;


    if (control?.valid) {
      return '';
    }

    for (const validator of validators) {
      if (control?.hasError(validator.name)) {
        return validator.message;
      }
    }

    return '';
  }

  public getValidationsFor(controlName: string): string[] {
    const control = this.formGroup.get(controlName) as FormControl;
    if (control.untouched) return [];

    const validators = this.controls[controlName];

    console.log("validators", validators);
    if (control?.valid) {
      return [];
    }

    let messages: string[] = [];
    for (const validator of validators) {
      if (control?.hasError(validator.name)) {
        messages.push(validator.message);
      }
    }

    console.log("messages", messages);
    return messages;
  }
}
