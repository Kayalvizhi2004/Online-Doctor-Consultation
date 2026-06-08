import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-input',
  templateUrl: './app-input.component.html'
})
export class AppInputComponent {

  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
}