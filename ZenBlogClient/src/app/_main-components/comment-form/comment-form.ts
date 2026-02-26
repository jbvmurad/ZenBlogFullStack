import { Component } from '@angular/core';
import { CommentService } from '../../_services/comment-service';
import { CommentDto } from '../../_models/commentDto';
import { ActivatedRoute } from '@angular/router';

declare const alertify: any;

@Component({
  selector: 'comment-form',
  standalone: false,
  templateUrl: './comment-form.html',
  styleUrl: './comment-form.css'
})
export class CommentForm {
  // Simple, explicit validation messages shown under inputs.
  // We intentionally avoid relying on Angular Forms directives here to prevent
  // "value not captured" issues caused by missing/incorrect module imports.
  errors: Record<string, string> = {};

  constructor(
    private commentService: CommentService,
    private route: ActivatedRoute
  ) {}

  private isEmail(email: string): boolean {
    // Practical email check (good enough for client-side validation)
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  }

  createComment(firstNameRaw: string, lastNameRaw: string, emailRaw: string, bodyRaw: string) {
    const firstName = (firstNameRaw ?? '').toString().trim();
    const lastName = (lastNameRaw ?? '').toString().trim();
    const email = (emailRaw ?? '').toString().trim();
    const body = (bodyRaw ?? '').toString().trim();

    // Reset errors
    this.errors = {};

    if (!firstName) this.errors['firstName'] = 'First name is required.';
    if (!lastName) this.errors['lastName'] = 'Last name is required.';
    if (!email) this.errors['email'] = 'Email is required.';
    else if (!this.isEmail(email)) this.errors['email'] = 'Email format is invalid.';
    if (!body) this.errors['body'] = 'Comment body is required.';

    if (Object.keys(this.errors).length > 0) {
      const msg = 'Form invalid. Please check: ' + Object.keys(this.errors).join(', ');
      if (typeof alertify !== 'undefined') alertify.error(msg);
      console.warn('[CommentForm] Form invalid', { firstName, lastName, email, body, errors: this.errors });
      return;
    }

    const blogId = this.route.snapshot.params['id'];
    if (!blogId) {
      const msg = 'Blog id not found in the route. Please refresh the page and try again.';
      if (typeof alertify !== 'undefined') alertify.error(msg);
      console.error('[CommentForm] Missing blogId from route params', this.route.snapshot.params);
      return;
    }

    const payload: CommentDto = {
      ...new CommentDto(),
      firstName,
      lastName,
      email,
      body,
      blogId
    };

    console.log('[CommentForm] Posting payload', payload);

    this.commentService.create(payload).subscribe({
      next: (res: any) => {
        if (typeof alertify !== 'undefined') {
          alertify.success(res?.message ?? 'Comment Posted!');
        }
        location.reload();
      },
      error: (err: any) => {
        console.error('[CommentForm] Comment Post Failed', err);
        if (typeof alertify !== 'undefined') {
          alertify.error(err?.error?.message ?? err?.error?.Message ?? 'Comment Post Failed!');
        }
      }
    });
  }
}
