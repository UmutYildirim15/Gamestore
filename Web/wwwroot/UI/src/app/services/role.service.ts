import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoaderService } from '../components/loader-component/loader.service';
import { appConfiguration } from '../configuration/configuration-resolver';
import { Role } from '../models/role.model';
import { BaseService } from './base.service';

@Injectable()
export class RoleService extends BaseService {
  constructor(http: HttpClient, loaderService: LoaderService) {
    super(http, loaderService);
  }

  getRole(id: string): Observable<Role> {
    return this.get<Role>(
      appConfiguration.roleApiUrl.replace(environment.routeIdIdentifier, id)
    );
  }

  getRoles(): Observable<Role[]> {
    return this.get<Role[]>(appConfiguration.rolesApiUrl);
  }

  getUserRoles(userId: string): Observable<Role[]> {
    return this.get<Role[]>(
      appConfiguration.userRolesApiUrl.replace(
        environment.routeIdIdentifier,
        userId
      )
    );
  }

  getRolePermissions(roleId: string): Observable<string[]> {
    if (!appConfiguration.rolePermissionsApiUrl?.length) {
      return of([]);
    }

    return this.get<string[]>(
      appConfiguration.rolePermissionsApiUrl.replace(
        environment.routeIdIdentifier,
        roleId
      )
    );
  }
  addRole(role: Role, permissions: string[]): Observable<any> {
    const newId = role.id || this.generateGuid();

    const payload = {
      id: newId,
      name: role.name,
      permissions
    };
    return this.post(appConfiguration.addRoleApiUrl, payload);
  }

  updateRole(role: Role, permissions: string[]): Observable<any> {
    const url = appConfiguration.updateRoleApiUrl!
        .replace('{id}', role.id!);

    const payload = {
      id: role.id,
      name: role.name,
      permissions
    };
    return this.put(url, payload);
  }

  deleteRole(id: string): Observable<any> {
    const url = appConfiguration.deleteRoleApiUrl.replace("{id}", id);
    return this.delete(url, {});
  }

  private generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
