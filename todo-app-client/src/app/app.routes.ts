import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import {
  CategoryListComponent
} from './features/categories/category-list/category-list.component';
import {
  TaskFormComponent
} from './features/tasks/task-form/task-form.component';
import {
  TaskListComponent
} from './features/tasks/task-list/task-list.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'tasks'
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [guestGuard],
    title: 'Login | Todo App'
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [guestGuard],
    title: 'Register | Todo App'
  },
  {
    path: 'tasks',
    component: TaskListComponent,
    canActivate: [authGuard],
    title: 'Tasks | Todo App'
  },
  {
    path: 'tasks/new',
    component: TaskFormComponent,
    canActivate: [authGuard],
    title: 'Add Task | Todo App'
  },
  {
    path: 'tasks/:id/edit',
    component: TaskFormComponent,
    canActivate: [authGuard],
    title: 'Edit Task | Todo App'
  },
  {
    path: 'categories',
    component: CategoryListComponent,
    canActivate: [authGuard],
    title: 'Categories | Todo App'
  },
  {
    path: '**',
    redirectTo: 'tasks'
  }
];