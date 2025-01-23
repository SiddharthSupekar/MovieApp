import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const loginGuard: CanActivateFn = (route, state) => {

  const verifylogin = sessionStorage.getItem("token");
  const router = inject(Router);

  if(verifylogin){
    return true;
  }
  else{
    router.navigateByUrl("/login")
    return false;
  }
};
