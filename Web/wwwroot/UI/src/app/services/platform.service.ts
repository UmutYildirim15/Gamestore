import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoaderService } from '../components/loader-component/loader.service';
import { appConfiguration } from '../configuration/configuration-resolver';
import { Platform } from '../models/platform.model';
import { BaseService } from './base.service';
import {map} from "rxjs/operators";

@Injectable()
export class PlatformService extends BaseService {
  constructor(http: HttpClient, loaderService: LoaderService) {
    super(http, loaderService);
  }

  getPlatform(id: string): Observable<Platform> {
    return this.get<Platform>(
      appConfiguration.platformApiUrl.replace(environment.routeIdIdentifier, id)
    );
  }

  getPlatformsByGameKey(gameKey: string): Observable<Platform[]> {
    return this.get<Platform[]>(
        appConfiguration.platformsByGameApiUrl.replace(environment.routeKeyIdentifier, gameKey)
    ).pipe(
        map(list => list.filter(p => p != null)) 
    );
  }

  getPlatforms(): Observable<Platform[]> {
    return this.get<Platform[]>(appConfiguration.platformsApiUrl);
  }

  addPlatform(platform: Platform): Observable<any> {
    const payload = {
      Platform: {
        Type: platform.type
      }
    };
    return this.post(appConfiguration.addPlatformApiUrl, payload);
  }

  updatePlatform(platform: Platform): Observable<any> {
    const url = appConfiguration.updatePlatformApiUrl!
        .replace('{id}', platform.id!); 

    const payload = {
      Platform: {
        Type: platform.type
      }
    };
    return this.put(url, payload);
  }


  deletePlatform(id: string): Observable<any> {
    return this.delete(
      appConfiguration.deletePlatformApiUrl.replace(
        environment.routeIdIdentifier,
        id
      ),
      {}
    );
  }
}
