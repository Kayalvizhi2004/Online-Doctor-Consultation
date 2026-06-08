import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="auth-wrapper">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    .auth-wrapper {
      min-height: 100vh;
      display: flex;
      justify-content: center;
      align-items: center;
      background: linear-gradient(120deg, #e3f2fd, #ffffff);
    }
  `]
})
export class AuthComponent {}