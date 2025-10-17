import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { ALL_PERMISSIONS, DEFAULT_PERMISSION_IDS } from 'src/app/configuration/permissions';
import { BaseComponent } from 'src/app/components/base.component';
import { Role } from 'src/app/models/role.model';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'gamestore-update-role',
  templateUrl: './update-role-page.component.html',
  styleUrls: ['./update-role-page.component.scss'],
})
export class UpdateRolePageComponent extends BaseComponent implements OnInit {
  form!: FormGroup;
  permissionItems = ALL_PERMISSIONS;         
  rolePermissions: string[] = [];

  constructor(
      private roleService: RoleService,
      private route: ActivatedRoute,
      private builder: FormBuilder,
      private router: Router
  ) { super(); }

  ngOnInit(): void {
    this.getRouteParam(this.route, 'id').pipe(
        switchMap(id => !!id ? this.roleService.getRole(id) : of(undefined)),
        switchMap(role => forkJoin({
          role: of(role),
          rolePermissions: role
              ? this.roleService.getRolePermissions(role.id!)
              : of<string[]>([])
        }))

    ).subscribe(({ role, rolePermissions }) => {
      this.rolePermissions = rolePermissions;
      this.initForm(role);
    });
  }

  private initForm(role?: Role) {
    this.form = this.builder.group({
      id: [role?.id ?? ''],
      name: [role?.name ?? '', Validators.required],
      permissions: this.builder.array(
          this.permissionItems.map(p => this.rolePermissions.includes(p.id))
      )
    });
  }

  get permissionsArray(): FormControl[] {
    return (this.form.get('permissions') as FormArray).controls as FormControl[];
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const role: Role = { id: this.form.value.id, name: this.form.value.name };
    let selectedIds = this.permissionItems
        .filter((_, i) => this.permissionsArray[i].value)
        .map(p => p.id);

    if (!selectedIds.length) {
      selectedIds = [...DEFAULT_PERMISSION_IDS];
    }

    const call$ = role.id
        ? this.roleService.updateRole(role, selectedIds)
        : this.roleService.addRole(role, selectedIds);

    call$.subscribe(() =>
        this.router.navigateByUrl(role.id ? `/roles/${role.id}` : '/roles')
    );
  }
}
