import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
declare const alertify :any;

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit  {
  protected readonly title = signal('ZenBlogClient');

  constructor(private router: Router) {}

  ngOnInit(): void {
    alertify.set('notifier','position','top-right');
    this.normalizeIncomingLinks();
  }

  private normalizeIncomingLinks() {
    if (typeof window === 'undefined') return;

    const href = window.location.href;
    const url = new URL(href);
    const path = url.pathname.toLowerCase();
    const hash = url.hash || '';
    let target: string | null = null;

    const aliasPathMap: Record<string, string> = {
      '/verifyemail': '/verify-email',
      '/confirmemail': '/verify-email',
      '/confirm-email': '/verify-email',
      '/resetpassword': '/reset-password'
    };

    if (hash.startsWith('#/')) {
      const hashValue = hash.slice(1);
      const [hashPathRaw, hashQueryRaw = ''] = hashValue.split('?');
      const hashPath = hashPathRaw.toLowerCase();

      if (hashPath === '/verify-email' || hashPath === '/verifyemail' || hashPath === '/confirm-email' || hashPath === '/confirmemail') {
        target = `/verify-email${hashQueryRaw ? `?${hashQueryRaw}` : ''}`;
      } else if (hashPath === '/reset-password' || hashPath === '/resetpassword') {
        target = `/reset-password${hashQueryRaw ? `?${hashQueryRaw}` : ''}`;
      }
    }

    if (!target && aliasPathMap[path]) {
      target = `${aliasPathMap[path]}${url.search}`;
    }

    if (!target && path === '/' && this.hasVerifyParams(url.searchParams)) {
      target = `/verify-email${url.search}`;
    }

    if (target && this.router.url !== target) {
      this.router.navigateByUrl(target, { replaceUrl: true });
    }
  }

  private hasVerifyParams(params: URLSearchParams): boolean {
    const hasUser = !!(params.get('userId') || params.get('id') || params.get('uid'));
    const hasToken = !!(params.get('token') || params.get('code') || params.get('emailToken'));
    return hasUser && hasToken;
  }
}
