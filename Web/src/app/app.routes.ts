import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserComponent } from './user/user.component';
import { AuthService } from './identity/auth.service';
import { inject } from '@angular/core';
import { TopUsersComponent } from './top-users/top-users.component';
import { SearchUsersComponent } from './search-users/search-users.component';
import { ProfileComponent } from './profile/profile.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
        title: 'Home'
    },
    {
        path: 'user/:username',
        component: UserComponent,
        // title is set in UserComponent
    },
    {
        path: 'login',
        component: LoginComponent,
        title: 'Login'
    },
    {
        path: 'register',
        component: RegisterComponent,
        title: 'Register'
    },
    {
        path: 'me',
        redirectTo: () => { 
            const authService = inject(AuthService);
            if (!authService.isSignedIn()) {
                return '/login';
            }
            // let username = '';
            // authService.getUsername().forEach((username => username)).then();
            return `/user/${authService.getUsername()}`
        },
        pathMatch: 'full'
    },
    {
        path: 'logout',
        redirectTo: () => {
            const authService = inject(AuthService);
            authService.signOut();
            return '/';
        },
    },
    {
        path: 'top',
        component: TopUsersComponent,
        title: 'Top viewed users'
    },
    {
        path: 'search',
        component: SearchUsersComponent,
        title: 'Search users'
    },
    {
        path: 'profile',
        component: ProfileComponent,
        title: 'Profile'
    }
];
