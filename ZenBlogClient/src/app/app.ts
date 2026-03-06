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

  alertify.set('notifier','position','top-right')

  // Some deployments (or older email templates) may generate hash-based links like:
  //   http://host/#/verify-email?userId=...&token=...
  // If the app uses PathLocationStrategy, Angular won't match routes from the hash.
  // This helper detects such links and forwards the user to the correct route.
  this.forwardHashDeepLinks();

}

private forwardHashDeepLinks() {
  try {
    const url = new URL(window.location.href);
    const hash = (url.hash ?? '').replace(/^#/, '').replace(/^!/, '');
    if (!hash) return;

    const normalized = hash.startsWith('/') ? hash.slice(1) : hash;
    const [hashPathRaw, hashQueryRaw] = normalized.split('?');
    const hashPath = (hashPathRaw ?? '').trim();
    if (!hashPath) return;

    const pathLower = hashPath.toLowerCase();

    const routeMap: Array<{ match: RegExp; target: string }> = [
      { match: /^(verify-email|verifyemail|confirm-email|confirmemail)$/i, target: 'verify-email' },
      { match: /^(reset-password|resetpassword)$/i, target: 'reset-password' }
    ];

    const hit = routeMap.find((r) => r.match.test(pathLower));
    if (!hit) return;

    const qp = new URLSearchParams(hashQueryRaw ?? '');
    const queryParams: any = {};
    qp.forEach((v, k) => (queryParams[k] = v));

    // Replace the URL so the user doesn't keep the old hash link in history.
    this.router.navigate(['/', hit.target], { queryParams, replaceUrl: true });
  } catch {
    // no-op
  }
}



}
