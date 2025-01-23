import { Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { LayoutComponent } from './Components/layout/layout.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { loginGuard } from './Guards/LoginGuard/login.guard';
import { NotFoundComponent } from './Components/not-found/not-found.component';

export const routes: Routes = [
    {
        path:'',
        component:LoginComponent
    },
    {
        path:'login',
        component:LoginComponent
    },
    {
        path:'register',
        component:RegisterComponent
    },
    {
        path:'layout',
        component:LayoutComponent,
        canActivate:[loginGuard],
        children:[
            {
                path:'dashboard',
                component:DashboardComponent
            }
        ]
    },
    {
        path:'**',
        component:NotFoundComponent
    }
];
