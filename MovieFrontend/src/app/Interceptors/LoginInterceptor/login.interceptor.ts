import { HttpInterceptorFn } from '@angular/common/http';

export const loginInterceptor: HttpInterceptorFn = (req, next) => {
  const cloneReq = req.clone({
    setHeaders:{
      Authorization: `Bearer ${sessionStorage.getItem("token")}`
    }
  })
  return next(cloneReq);
};
