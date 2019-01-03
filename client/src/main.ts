import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { extendArrayType } from './app/helpers/array-extensions';

if (environment.production) {
  enableProdMode();
}

extendArrayType();

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));

