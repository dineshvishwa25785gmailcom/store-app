import { CanActivateFn, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../_service/user.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  let router = inject(Router);
  let toastr = inject(ToastrService);
  let service = inject(UserService);

  let menuname = '';

  if (route.url.length > 0) {
    menuname = route.url[0].path;
    // Map editinvoice to createinvoice for permission check
    if (menuname === 'editinvoice') {
      menuname = 'createinvoice';
    }
  }

  if (localStorage.getItem('username') != null) {
    let userrole = localStorage.getItem('userrole') as string;
    if (menuname != '') {
      service.Getmenupermission(userrole, menuname).subscribe((item) => {
        if (item.haveview) {
          return true;
        } else {
          toastr.warning('Unauthorized access');
          router.navigateByUrl('/');
          return false;
        }
      });
      return true;
    } else {
      return true;
    }
  } else {
    toastr.warning('Unauthorized access');
    router.navigateByUrl('/login');
    return false;
  }
};
