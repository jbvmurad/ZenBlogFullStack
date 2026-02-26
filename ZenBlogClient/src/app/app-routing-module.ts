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
import { AuthGuard } from './_guards/auth-guard';
import { Blogdetails } from './_main-components/blogdetails/blogdetails';
import { ContactMain } from './_main-components/contact-main/contact-main';
import { Comment } from './_admin-components/comment/comment';
import { ContactInfo } from './_admin-components/contact-info/contact-info';
import { Message } from './_admin-components/message/message';
import { Social } from './_admin-components/social/social';


const routes: Routes = [

//Main Routes

{ path:'', component: MainLayout,

  children: [
    {path:'', component:Home},
    {path:'login',component:Login},
    {path:'register',component:Register},
    {path:'verify-email',component:VerifyEmail},
    {path:'blogdetails/:id',component:Blogdetails},
    {path:'contact',component:ContactMain}
  ]
},



//Admin Routes    http://localhost:4200/admin/category

{path:'admin',
  component:AdminLayout,
  canActivate:[AuthGuard],
  children:[
    {path:'category',
      component:Category,
    canActivate:[AuthGuard]},
    {path:'blog',
       component:Blog,
      canActivate:[AuthGuard]},
       {path:'comment',
       component:Comment,
      canActivate:[AuthGuard]},
      {path:'contactinfo',
       component:ContactInfo,
      canActivate:[AuthGuard]},
       {path:'message',
       component:Message,
      canActivate:[AuthGuard]},
       {path:'social',
       component:Social,
      canActivate:[AuthGuard]}

  ]
}


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
