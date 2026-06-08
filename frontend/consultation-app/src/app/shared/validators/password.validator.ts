import { AbstractControl } from '@angular/forms';

export function passwordValidator(control: AbstractControl) {

  const value = control.value;

  const hasNumber = /\d/.test(value);
  const hasUpper = /[A-Z]/.test(value);
  const hasLength = value?.length >= 8;

  if (hasNumber && hasUpper && hasLength) return null;

  return { weakPassword: true };
}