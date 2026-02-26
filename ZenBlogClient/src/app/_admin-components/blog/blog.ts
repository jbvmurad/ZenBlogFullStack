import { Component, OnInit } from '@angular/core';
import { BlogService } from '../../_services/blog-service';
import { SweetalertService } from '../../_services/sweetalert-service';
import { BlogDto } from '../../_models/blog';
import { CategoryService } from '../../_services/category-service';
import { AuthService } from '../../_services/auth-service';
declare const alertify:any;

@Component({
  selector: 'app-blog',
  standalone: false,
  templateUrl: './blog.html',
  styleUrl: './blog.css'
})
export class Blog implements OnInit {

  constructor(
    private blogService:BlogService,
    private swal: SweetalertService,
    private categoryService: CategoryService,
    private authService: AuthService
  ){}

  blogs:BlogDto[];
  categories: any= [];
  newBlog: BlogDto= new BlogDto();
  editBlog:any ={};
  private originalEditBlog:any = null;
  errors:any= [];

  // Store-style file pickers
  newCoverFile: File | null = null;
  newBlogFile: File | null = null;
  editCoverFile: File | null = null;
  editBlogFile: File | null = null;

  ngOnInit(): void {
    this.getBlogs();
    this.getCategories();
  }

  getBlogs(){
    this.blogService.getAll().subscribe({
      next: result => this.blogs= result,
      error: err => {
        console.error(err);
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
      }
    })
  }

  onNewCoverSelected(e: any){
    this.newCoverFile = e?.target?.files?.[0] ?? null;
  }
  onNewBlogSelected(e: any){
    this.newBlogFile = e?.target?.files?.[0] ?? null;
  }
  onEditCoverSelected(e: any){
    this.editCoverFile = e?.target?.files?.[0] ?? null;
  }
  onEditBlogSelected(e: any){
    this.editBlogFile = e?.target?.files?.[0] ?? null;
  }

  create(){
    this.errors= {};

    this.newBlog.userId = this.authService.getUserId();

    if (!this.newCoverFile || !this.newBlogFile) {
      alertify.error('Please choose both Cover Image and Blog Image files.');
      return;
    }

    this.blogService.create(this.newBlog, this.newCoverFile, this.newBlogFile).subscribe({
      next: (res: any) => {
        alertify.success(res?.message ?? "Blog Created!");
        this.getBlogs();
      },
      error : result => {
        alertify.error(result?.error?.message ?? result?.error?.Message ?? result?.message ?? "An Error Occured!");
        this.errors= result.error?.errors;
      },
      complete: () => {
        this.errors= {};
        this.newCoverFile = null;
        this.newBlogFile = null;
      }
    })
  }

  getCategories(){
    this.categoryService.getCategories().subscribe({
      next: result => this.categories= result,
      error: err => console.error(err)
    })
  }

  onSelected(blog: any){
    this.errors= {};
    // Work on a copy so we can compare changes and avoid mutating the list item directly
    this.editBlog= { ...blog };
    this.originalEditBlog = { ...blog };
    this.editCoverFile = null;
    this.editBlogFile = null;
  }

  update(){
    const changes: any = { id: this.editBlog?.id };

    // Only send changed fields (server will preserve omitted/empty ones)
    if (this.originalEditBlog) {
      if (this.editBlog?.title !== this.originalEditBlog?.title) changes.title = this.editBlog?.title;
      if (this.editBlog?.description !== this.originalEditBlog?.description) changes.description = this.editBlog?.description;
      if (this.editBlog?.categoryId !== this.originalEditBlog?.categoryId) changes.categoryId = this.editBlog?.categoryId;
      // Intentionally do NOT overwrite userId on update unless you want to change author
      // if (this.editBlog?.userId !== this.originalEditBlog?.userId) changes.userId = this.editBlog?.userId;
    }

    this.blogService.update(changes, this.editCoverFile, this.editBlogFile).subscribe({
      next: (res: any) => {
        alertify.success(res?.message ?? "Blog Updated!");
        this.getBlogs();
        this.errors = {};
      },
      error: err =>{
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!");
        this.errors = err.error?.errors
      },
      complete: () => {
        this.editCoverFile = null;
        this.editBlogFile = null;
        // refresh baseline after update
        this.originalEditBlog = { ...this.editBlog };
      }
    })
  }

  async delete(id: string){
    const isConfirmed = await this.swal.areYouSure();

    if(isConfirmed){
      this.blogService.delete(id).subscribe({
        next: (res: any) => {
          alertify.success(res?.message ?? "Blog Deleted!");
          this.getBlogs();
        },
        error: err => {
          console.error(err);
          alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!");
        }
      })
    }
    else{
      console.log("Delete Reverted")
    }
  }
}
