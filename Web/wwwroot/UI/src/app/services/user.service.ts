import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LoaderService } from '../components/loader-component/loader.service';
import { appConfiguration } from '../configuration/configuration-resolver';
import { User } from '../models/user.model';
import { BaseService } from './base.service';

@Injectable()
export class UserService extends BaseService {
  constructor(http: HttpClient, loaderService: LoaderService) {
    super(http, loaderService);
  }

  getUser(id: string): Observable<User> {
    return this.get<User>(
        appConfiguration.userApiUrl.replace(environment.routeIdIdentifier, id)
    );
  }

  getUsers(): Observable<User[]> {
    return this.get<User[]>(appConfiguration.usersApiUrl);
  }

  addUser(user: User, roles: string[], password: string): Observable<any> {
    const userId = user.id && user.id.trim() ? user.id : this.generateGuid();

    const payload = {
      id: userId,
      name: user.name,
      password,
      roleIds: roles
    };
    return this.post(appConfiguration.addUserApiUrl, payload);
  }

  updateUser(user: User, roles: string[], password: string): Observable<any> {
    const baseUrl = appConfiguration.updateUserApiUrl || '';

    const payload = {
      id: user.id,
      name: user.name,
      password,
      roleIds: roles
    };
    return this.put(baseUrl, payload);
  }

  private generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  login(model: {
    login: string;
    password: string;
    internalAuth: boolean;
  }): Observable<{ token: string }> {
    return this.post<any, { token: string }>(appConfiguration.loginApiUrl, model).pipe(
        tap((x) => {
          const token = x.token;
          this.clearAuth();
          if (!token?.length) {
            return;
          }
          localStorage.setItem('authKey', token);
        })
    );
  }

  deleteUser(id: string): Observable<any> {
    return this.delete(
        appConfiguration.deleteUserApiUrl.replace(
            environment.routeIdIdentifier,
            id
        ),
        {}
    );
  }

  checkAccess(targetPage: string, targetId?: string): Observable<boolean> {
    return this.post(appConfiguration.checkAccessApiUrl, {
      targetPage,
      targetId,
    });
  }

  clearAuth(): void {
    localStorage.removeItem('authKey');
  }

  isAuth(): boolean {
    return !!localStorage.getItem('authKey')?.length;
  }

  getCurrentUserId(): string {
    const token = localStorage.getItem('authKey');
    if (!token) return '';
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub || payload.nameid || '';
    } catch {
      return '';
    }
  }
}