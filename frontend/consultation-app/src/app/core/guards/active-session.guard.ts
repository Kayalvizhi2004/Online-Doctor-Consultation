import { CanDeactivateFn } from '@angular/router';
import { CanComponentDeactivate } from '../../features/chat/pages/consultation-chat/consultation-chat.component';

/**
 * Blocks a patient from navigating away from a live consultation until the
 * doctor ends it. The component decides via canDeactivate() (doctors and
 * ended sessions are always allowed to leave).
 */
export const activeSessionGuard: CanDeactivateFn<CanComponentDeactivate> = (component) => {
  if (component.canDeactivate()) return true;
  alert('Please wait for the doctor to end the consultation before leaving.');
  return false;
};
