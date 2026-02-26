import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { AdminLayout } from './_layouts/admin-layout/admin-layout';
import { MainLayout } from './_layouts/main-layout/main-layout';
import { Home } from './_main-components/home/home';
import { Category } from './_admin-components/category/category';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Blog } from './_admin-components/blog/blog';
import { Login } from './_main-components/login/login';
import { Register } from './_main-components/register/register';
import { VerifyEmail } from './_main-components/verify-email/verify-email';
import { JwtHelperService, JwtModule } from '@auth0/angular-jwt';
import { AuthGuard } from './_guards/auth-guard';
import { Blogdetails } from './_main-components/blogdetails/blogdetails';
import { CommentForm } from './_main-components/comment-form/comment-form';
import { ContactMain } from './_main-components/contact-main/contact-main';
import { Comment } from './_admin-components/comment/comment';
import { ContactInfo } from './_admin-components/contact-info/contact-info';
import { TokenInterceptor } from './_interceptors/token-interceptor';
import { ErrorInterceptor } from './_interceptors/error-interceptor';
import { Message } from './_admin-components/message/message';
import { Social } from './_admin-components/social/social';
import { SendMessage } from './_main-components/send-message/send-message';

@NgModule({
  declarations: [
    App,
    AdminLayout,
    MainLayout,
    Home,
    Category,
    Blog,
    Login,
    Register,
    VerifyEmail,
    Blogdetails,
    CommentForm,
    ContactMain,
    Comment,
    ContactInfo,
    Message,
    Social,
    SendMessage
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule

  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    {provide: HTTP_INTERCEPTORS,useClass: TokenInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS,useClass: ErrorInterceptor, multi: true}

  ],
  bootstrap: [App]
})
export class AppModule { }
