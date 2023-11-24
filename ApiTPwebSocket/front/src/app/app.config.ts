import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { PixelService } from '../services/PixelService.service';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    { provide: PixelService, useClass: PixelService }
  ]
};
