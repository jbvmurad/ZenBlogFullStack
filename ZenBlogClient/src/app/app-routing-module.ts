import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainLayout } from './_layouts/main-layout/main-layout';
import { Home } from './_main-components/home/home';
import { AdminLayout } from './_layouts/admin-layout/admin-layout';
import { Category } from './_admin-components/category/category';
import { Blog } from './_admin-components/blog/blog';
import { Login } from './_main-components/login/login';
import { Register } from './_main-components/register/register';
import { VerifyEmail } from './_main-components/verify-email/verify-email';
import { ForgotPassword } from './_main-components/forgot-password/forgot-password';
import { ResetPassword } from './_main-components/reset-password/reset-password';
import { AuthGuard } from './_guards/auth-guard';
import { Blogdetails } from './_main-components/blogdetails/blogdetails';
import { ContactMain } from './_main-components/contact-main/contact-main';
import { Comment } from './_admin-components/comment/comment';
import { ContactInfo } from './_admin-components/contact-info/contact-info';
import { Message } from './_admin-components/message/message';
import { Social } from './_admin-components/social/social';
import { Profile } from './_main-components/profile/profile';
import { Settings } from './_main-components/settings/settings';
import { AdminGuard } from './_guards/admin-guard';
import { GuestGuard } from './_guards/guest-guard';
import { UserRoles } from './_admin-components/user-roles/user-roles';


const routes: Routes = [

//Main Routes

{ path:'', component: MainLayout,

  children: [
    {path:'', component:Home},
    {path:'login',component:Login, canActivate:[GuestGuard]},
    {path:'register',component:Register, canActivate:[GuestGuard]},
    {path:'forgot-password',component:ForgotPassword},
    {path:'reset-password',component:ResetPassword},
    {path:'verify-email',component:VerifyEmail},

    // Aliases (tolerate different link formats coming from emails)
    { path: 'verifyEmail', redirectTo: 'verify-email', pathMatch: 'full' },
    { path: 'VerifyEmail', redirectTo: 'verify-email', pathMatch: 'full' },
    { path: 'confirm-email', redirectTo: 'verify-email', pathMatch: 'full' },
    { path: 'confirmEmail', redirectTo: 'verify-email', pathMatch: 'full' },

    { path: 'resetPassword', redirectTo: 'reset-password', pathMatch: 'full' },
    { path: 'ResetPassword', redirectTo: 'reset-password', pathMatch: 'full' },

    {path:'blogdetails/:id',component:Blogdetails},
    {path:'contact',component:ContactMain},

    // Account
    {path:'profile',component:Profile, canActivate:[AuthGuard]},
    {path:'settings',component:Settings, canActivate:[AuthGuard]}
  ]
},



//Admin Routes    http://localhost:4200/admin/category

{path:'admin',
  component:AdminLayout,
  canActivate:[AdminGuard],
  children:[
    { path: '', redirectTo: 'category', pathMatch: 'full' },
    {path:'category',
      component:Category,
    },
    {path:'blog',
       component:Blog,
      },
       {path:'comment',
       component:Comment,
      },
      {path:'contactinfo',
       component:ContactInfo,
      },
       {path:'message',
       component:Message,
      },
       {path:'social',
       component:Social,
      },

      // Role assignment (admin only)
      {path:'users', component:UserRoles}

  ]
}

// Fallback
,{ path: '**', redirectTo: '' }


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
