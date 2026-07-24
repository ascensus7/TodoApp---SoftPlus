import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TokenStorageService } from '../services/token-storage.service';

export const authInterceptor: HttpInterceptorFn =
  (request, next) => {
    const tokenStorage = inject(TokenStorageService);
    const token = tokenStorage.getToken();

    const isOurApi =
      request.url.startsWith(environment.apiUrl);

    if (token === null || isOurApi === false) {
      return next(request);
    }

    const requestWithToken = request.clone({
      setHeaders: {
        Authorization: 'Bearer ' + token
      }
    });

    return next(requestWithToken);
  };