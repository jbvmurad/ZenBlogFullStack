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
    const normalizedPath = this.normalizePath(url.pathname);
    const hash = url.hash || '';
    let target: string | null = null;

    const aliasPathMap: Record<string, string> = {
      '/verifyemail': '/verify-email',
      '/verify-email': '/verify-email',
      '/confirmemail': '/verify-email',
      '/confirm-email': '/verify-email',
      '/resetpassword': '/reset-password',
      '/reset-password': '/reset-password'
    };

    if (hash.startsWith('#/')) {
      const hashValue = hash.slice(1);
      const [hashPathRaw, hashQueryRaw = ''] = hashValue.split('?');
      const hashPath = this.normalizePath(hashPathRaw);

      if (aliasPathMap[hashPath]) {
        target = `${aliasPathMap[hashPath]}${hashQueryRaw ? `?${hashQueryRaw}` : ''}`;
      }
    }

    if (!target && aliasPathMap[normalizedPath]) {
      target = `${aliasPathMap[normalizedPath]}${url.search}`;
    }

    if (!target && normalizedPath === '/' && this.hasVerifyParams(url.searchParams)) {
      target = `/verify-email${url.search}`;
    }

    if (!target && normalizedPath === '/' && this.hasResetParams(url.searchParams)) {
      target = `/reset-password${url.search}`;
    }

    if (target && this.router.url !== target) {
      this.router.navigateByUrl(target, { replaceUrl: true });
    }
  }

  private normalizePath(value: string): string {
    if (!value) return '/';
    const lower = value.toLowerCase();
    const trimmed = lower.length > 1 ? lower.replace(/\/+$/, '') : lower;
    return trimmed || '/';
  }

  private hasVerifyParams(params: URLSearchParams): boolean {
    const hasUser = !!(params.get('userId') || params.get('id') || params.get('uid'));
    const hasToken = !!(params.get('token') || params.get('code') || params.get('emailToken'));
    return hasUser && hasToken;
  }

  private hasResetParams(params: URLSearchParams): boolean {
    const hasUser = !!(params.get('userId') || params.get('id') || params.get('uid'));
    const hasToken = !!(params.get('token') || params.get('code') || params.get('resetToken'));
    return hasUser && hasToken;
  }
}
