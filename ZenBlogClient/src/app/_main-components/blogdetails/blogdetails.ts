import { Component } from '@angular/core';
import { BlogService } from '../../_services/blog-service';
import { ActivatedRoute } from '@angular/router';
import { BlogDto } from '../../_models/blog';
import { CommentDto } from '../../_models/commentDto';
import { CommentService } from '../../_services/comment-service';

@Component({
  selector: 'app-blogdetails',
  standalone: false,
  templateUrl: './blogdetails.html',
  styleUrl: './blogdetails.css'
})
export class Blogdetails {
  blog: BlogDto = { ...new BlogDto(), comments: [], user: {}, category: {} } as BlogDto;
  latestBlogs: BlogDto[] = [];

  constructor(
    private blogService: BlogService,
    private route: ActivatedRoute,
    private commentService: CommentService
  ){
    this.getBlogById();
    this.getLatestBlogs();
  }

  getBlogById(){
    const blogId = this.route.snapshot.params['id'];
    this.blogService.getBlogById(blogId).subscribe({
      next: result => {
        this.blog = result ?? ({ ...new BlogDto(), comments: [], user: {}, category: {} } as BlogDto);
        this.getComments(blogId);
      }
    })
  }

  getLatestBlogs(){
    this.blogService.getLatest5Blogs().subscribe({
      next: result => this.latestBlogs = Array.isArray(result) ? result : []
    })
  }

  getComments(blogId: string) {
    this.commentService.getForBlog(blogId).subscribe({
      next: result => {
        this.blog.comments = Array.isArray(result) ? result : [];
      },
      error: () => {
        this.blog.comments = [];
      }
    });
  }

  refreshComments() {
    const blogId = this.route.snapshot.params['id'];
    if (!blogId) return;

    this.getComments(blogId);
  }

  commentDisplayName(comment: CommentDto): string {
    return `${comment.firstName ?? ''} ${comment.lastName ?? ''}`.trim() || comment.email || 'Comment user';
  }

  commentInitials(comment: CommentDto): string {
    return this.commentDisplayName(comment)
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(value => value[0])
      .join('')
      .toUpperCase();
  }
}
