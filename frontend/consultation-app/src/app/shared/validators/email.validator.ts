import { AbstractControl } from '@angular/forms';

export function emailValidator(control: AbstractControl) {

  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  return regex.test(control.value)
    ? null
    : { invalidEmail: true };
}