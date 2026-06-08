import { AbstractControl } from '@angular/forms';

export function phoneValidator(control: AbstractControl) {

  const regex = /^[0-9]{10}$/;

  return regex.test(control.value)
    ? null
    : { invalidPhone: true };
}