import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoaderService } from '../components/loader-component/loader.service';
import { appConfiguration } from '../configuration/configuration-resolver';
import { Game } from '../models/game.model';
import { BaseService } from './base.service';
import {map} from "rxjs/operators";

@Injectable()
export class GameService extends BaseService {
  constructor(http: HttpClient, loaderService: LoaderService) {
    super(http, loaderService);
  }

  getGame(key: string): Observable<Game> {
    return this.get<Game>(
      appConfiguration.gameApiUrl.replace(environment.routeKeyIdentifier, key)
    );
  }

  getGameById(id: string): Observable<Game> {
    return this.get<Game>(
      appConfiguration.gameByIdApiUrl.replace(environment.routeIdIdentifier, id)
    );
  }

  getGames(filterParams: HttpParams): Observable<{
    games: Game[];
    totalPages: number;
    currentPage: number;
  }> {
    return this.getWithParams<{
      items: Game[];
      totalPages: number;
      page: number;
    }>(appConfiguration.gamesApiUrl, filterParams).pipe(
        map(resp => ({
          games: resp.items,
          totalPages: resp.totalPages,
          currentPage: resp.page
        }))
    );
  }


  getAllGames(): Observable<Game[]> {
    return this.get(appConfiguration.gamesAllApiUrl);
  }

  addGame(
    game: Game,
    genres: string[],
    platforms: string[],
    publisher: string
  ): Observable<any> {
    return this.post(appConfiguration.addGameApiUrl, {
      game,
      genres,
      platforms,
      publisher,
    });
  }

  updateGame(
      game: Game,
      genres: string[],
      platforms: string[],
      publisher: string
  ): Observable<any> {
    const urlTemplate = appConfiguration.updateGameApiUrl;
    const url = urlTemplate.replace(environment.routeIdIdentifier, game.id!);
    return this.put(url, { game, genres, platforms, publisher });
  }

  deleteGame(key: string): Observable<any> {
    return this.delete(
      appConfiguration.deleteGameApiUrl.replace(
        environment.routeKeyIdentifier,
        key
      ),
      {}
    );
  }

  getGamesByGenre(genreId: string): Observable<Game[]> {
    return this.get<Game[]>(
      appConfiguration.gamesByGenreApiUrl.replace(
        environment.routeIdIdentifier,
        genreId
      )
    );
  }

  getGamesByPlatfrom(platformId: string): Observable<Game[]> {
    return this.get<Game[]>(
      appConfiguration.gamesByPlatformApiUrl.replace(
        environment.routeIdIdentifier,
        platformId
      )
    );
  }

  getGamesByPublisher(publisherId: string): Observable<Game[]> {
    return this.get<Game[]>(
      appConfiguration.gamesByPublisherApiUrl.replace(
        environment.routeIdIdentifier,
        publisherId
      )
    );
  }

  getGameFile(key: string): Observable<Blob> {
    return this.getFile(
      appConfiguration.getGameFileApiUrl.replace(
        environment.routeKeyIdentifier,
        key
      )
    );
  }

  getPublishedDateOptions(): Observable<string[]> {
    return this.get(appConfiguration.getPublishedDateOptionsApiUrl);
  }

  getSortingOptions(): Observable<string[]> {
    return this.get(appConfiguration.getSortingOptionsApiUrl);
  }

  getPaggingOptions(): Observable<string[]> {
    return this.get(appConfiguration.getPaggingOptionsApiUrl);
  }
}
