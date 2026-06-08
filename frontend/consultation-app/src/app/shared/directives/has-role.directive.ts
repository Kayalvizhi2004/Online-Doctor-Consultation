import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Directive({ selector: '[hasRole]' })
export class HasRoleDirective {
  private auth = inject(AuthService);
  private currentUserRole: string | undefined;

  constructor(private tpl: TemplateRef<any>, private vc: ViewContainerRef) {
    this.currentUserRole = this.auth.getUserRole?.() as string | undefined;
  }

  @Input()
  set hasRole(role: string) {
    const allowed = role === this.currentUserRole;
    this.vc.clear();
    if (allowed) this.vc.createEmbeddedView(this.tpl);
  }
}
