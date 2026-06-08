import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Directive({
  selector: '[appRole]'
})
export class RoleDirective {

  constructor(
    private templateRef: TemplateRef<any>,
    private vcr: ViewContainerRef,
    private auth: AuthService
  ) {}

  @Input() set appRole(role: string) {

    const userRole = this.auth.getUserRole();

    if (role === userRole) {
      this.vcr.createEmbeddedView(this.templateRef);
    } else {
      this.vcr.clear();
    }
  }
}