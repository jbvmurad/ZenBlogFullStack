import { Injectable } from '@angular/core';
import { GOOGLE_CLIENT_ID } from '../_configs/google-auth';

declare const google: any;

@Injectable({
  providedIn: 'root'
})
export class GoogleIdentityService {
  private scriptPromise: Promise<void> | null = null;

  loadClient(): Promise<void> {
    if (typeof window === 'undefined' || typeof document === 'undefined') {
      return Promise.reject(new Error('Google Identity can only run in the browser.'));
    }

    if ((window as any).google?.accounts?.id) {
      return Promise.resolve();
    }

    if (this.scriptPromise) {
      return this.scriptPromise;
    }

    this.scriptPromise = new Promise<void>((resolve, reject) => {
      const existingScript = document.querySelector('script[data-google-identity="true"]') as HTMLScriptElement | null;

      const finishWhenReady = () => {
        let checks = 0;
        const maxChecks = 150;
        const timer = window.setInterval(() => {
          if ((window as any).google?.accounts?.id) {
            window.clearInterval(timer);
            resolve();
            return;
          }

          checks += 1;
          if (checks >= maxChecks) {
            window.clearInterval(timer);
            reject(new Error('Google Identity script did not finish loading in time.'));
          }
        }, 200);
      };

      if (existingScript) {
        existingScript.addEventListener('load', finishWhenReady, { once: true });
        existingScript.addEventListener('error', () => reject(new Error('Google Identity script failed to load.')), { once: true });
        finishWhenReady();
        return;
      }

      const script = document.createElement('script');
      script.src = 'https://accounts.google.com/gsi/client';
      script.async = true;
      script.defer = true;
      script.dataset['googleIdentity'] = 'true';
      script.addEventListener('load', finishWhenReady, { once: true });
      script.addEventListener('error', () => reject(new Error('Google Identity script failed to load.')), { once: true });
      document.head.appendChild(script);
    });

    return this.scriptPromise;
  }

  async renderButton(containerId: string, buttonText: 'signin_with' | 'signup_with', callback: (response: any) => void): Promise<boolean> {
    await this.loadClient();

    const container = await this.waitForContainer(containerId);
    if (!container || typeof google === 'undefined' || !google?.accounts?.id) {
      return false;
    }

    container.innerHTML = '';

    google.accounts.id.initialize({
      client_id: GOOGLE_CLIENT_ID,
      callback
    });

    google.accounts.id.renderButton(container, {
      theme: 'outline',
      size: 'large',
      shape: 'pill',
      text: buttonText,
      width: 360
    });

    return true;
  }

  private async waitForContainer(containerId: string): Promise<HTMLElement | null> {
    for (let i = 0; i < 60; i += 1) {
      const container = document.getElementById(containerId);
      if (container) {
        return container;
      }

      await new Promise(resolve => window.setTimeout(resolve, 100));
    }

    return null;
  }
}
