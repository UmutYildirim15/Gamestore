import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable} from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoaderService } from '../components/loader-component/loader.service';
import { appConfiguration } from '../configuration/configuration-resolver';
import { Genre } from '../models/genre.model';
import { BaseService } from './base.service';

@Injectable()
export class GenreService extends BaseService {
  constructor(http: HttpClient, loaderService: LoaderService) {
    super(http, loaderService);
  }

  getGenre(id: string): Observable<Genre> {
    return this.get<Genre>(
      appConfiguration.genreApiUrl.replace(environment.routeIdIdentifier, id)
    );
  }

  getGenres(): Observable<Genre[]> {
    return this.get<Genre[]>(appConfiguration.genresApiUrl);
  }

  getGenresByGameKey(gameKey: string): Observable<Genre[]> {
    return this.get<Genre[]>(
      appConfiguration.genresByGameApiUrl.replace(
        environment.routeKeyIdentifier,
        gameKey
      )
    );
  }

  getGenresByParent(id: string): Observable<Genre[]> {
    return this.get<Genre[]>(
      appConfiguration.genresByParentApiUrl.replace(
        environment.routeIdIdentifier,
        id
      )
    );
  }

  addGenre(genre: Genre): Observable<any> {
    const url = appConfiguration.addGenreApiUrl!.replace("{id}", genre.id!);
    

    const payload = {
      Genre: {
        Name: genre.name,
        ParentGenreId: genre.parentGenreId
      }
    };

    return this.post(url, payload);
  }


  updateGenre(genre: Genre): Observable<any> {
    const url = appConfiguration.updateGenreApiUrl!
        .replace("{id}", genre.id!);

    const payload = {
      Genre: {
        Name: genre.name,
        ParentGenreId: genre.parentGenreId
      }
    };

    return this.put(url, payload);
  }


  deleteGenre(id: string): Observable<any> {
    return this.delete(
      appConfiguration.deleteGenreApiUrl.replace(
        environment.routeIdIdentifier,
        id
      ),
      {}
    );
  }
}
