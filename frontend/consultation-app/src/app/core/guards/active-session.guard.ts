import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CanComponentDeactivate } from '../../features/chat/pages/consultation-chat/consultation-chat.component';

export const activeSessionGuard: CanDeactivateFn<CanComponentDeactivate> = (component) => {

  const toastr = inject(ToastrService);

  if (component.canDeactivate()) {
    return true;
  }

  toastr.info(
    'Please wait for the doctor to end the consultation before leaving.',
    'Consultation Active'
  );

  return false;
};
