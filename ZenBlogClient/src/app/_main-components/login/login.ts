import { AfterViewInit, Component } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { LoginDto } from '../../_models/loginDto';
import { Router } from '@angular/router';
import { GOOGLE_CLIENT_ID } from '../../_configs/google-auth';
declare const alertify:any;
declare const google: any;

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements AfterViewInit {

constructor(private authService:AuthService,
  private router: Router
){}

loginDto: LoginDto = new LoginDto()
token :any;
decodedToken: any;


login(){
this.authService.login(this.loginDto).subscribe({
  next: result => {
    const token =
      result?.Token ??
      result?.token ??
      result?.data?.Token ??
      result?.data?.token;
    this.token = token;
    if (!token) {
      alertify.error("Login Failed! (Token not found in response)");
      return;
    }
    localStorage.setItem("token", token);

          alertify.success("Login Successful!");
          this.router.navigate(["/admin"])


  },
  error: result => {
    const msg =
      result?.error?.Message ??
      result?.error?.message ??
      (result?.error?.errors ? JSON.stringify(result.error.errors) : null) ??
      'Login Failed!';
    alertify.error(msg);
  }
})

}

ngAfterViewInit(): void {
  this.renderGoogleButtonWithRetry();
}

private renderGoogleButtonWithRetry(triesLeft: number = 20) {
  const el = document.getElementById('googleBtn');
  if (!el) return;

  if (typeof google === 'undefined' || !google?.accounts?.id) {
    if (triesLeft <= 0) return;
    setTimeout(() => this.renderGoogleButtonWithRetry(triesLeft - 1), 200);
    return;
  }

  try {
    google.accounts.id.initialize({
      client_id: GOOGLE_CLIENT_ID,
      callback: (response: any) => this.handleGoogleCredential(response)
    });

    google.accounts.id.renderButton(el, {
      theme: 'outline',
      size: 'large',
      shape: 'pill',
      text: 'signin_with',
      width: 360
    });
  } catch {
    // no-op
  }
}

private handleGoogleCredential(response: any) {
  const credential = response?.credential;
  if (!credential) {
    alertify.error('Google sign-in failed (no credential).');
    return;
  }

  this.authService.loginWithGoogle(credential).subscribe({
    next: (result) => {
      const token =
        result?.Token ??
        result?.token ??
        result?.data?.Token ??
        result?.data?.token;

      if (!token) {
        alertify.error('Google sign-in failed (Token not found in response).');
        return;
      }

      localStorage.setItem('token', token);
      alertify.success('Signed in with Google!');
      this.router.navigate(['/admin']);
    },
    error: (err) => {
      // Try to surface the real backend/network error (Angular sometimes gives err.error as string or ProgressEvent)
      let parsedMsg: string | null = null;
      if (typeof err?.error === 'string') {
        try {
          const obj = JSON.parse(err.error);
          parsedMsg = obj?.Message ?? obj?.message ?? null;
        } catch {
          parsedMsg = err.error;
        }
      }

      const msg =
        err?.error?.Message ??
        err?.error?.message ??
        parsedMsg ??
        (typeof parsedMsg === 'string' && /untrusted\s+'aud'|audience/i.test(parsedMsg)
          ? `Google token audience uyuşmuyor. Backend'de GoogleAuth:ClientId değeri, client'daki GOOGLE_CLIENT_ID ile aynı olmalı. (Client: ${GOOGLE_CLIENT_ID})`
          : null) ??
        (err?.status === 0
          ? 'Google sign-in failed: API reachedilemedi. proxy.conf.json target adresini ve backend\'in çalıştığını kontrol et.'
          : null) ??
        `Google sign-in failed. (HTTP ${err?.status ?? 'unknown'})`;

      alertify.error(msg);
    }
  });
}


decodeToken(){
let decodedToken =  this.authService.decodeToken();
this.decodedToken= decodedToken;
}


}
