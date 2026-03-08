import { Component, OnDestroy, OnInit } from '@angular/core';
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
export class Blog implements OnInit, OnDestroy {

  constructor(
    private blogService:BlogService,
    private swal: SweetalertService,
    private categoryService: CategoryService,
    private authService: AuthService
  ){}

  blogs: BlogDto[] = [];
  categories: any[] = [];
  newBlog: BlogDto = new BlogDto();
  editBlog:any ={};
  private originalEditBlog:any = null;
  errors:any= [];
  searchTerm = '';

  newCoverFile: File | null = null;
  newBlogFile: File | null = null;
  editCoverFile: File | null = null;
  editBlogFile: File | null = null;

  newCoverPreviewUrl: string | null = null;
  newBlogPreviewUrl: string | null = null;
  editCoverPreviewUrl: string | null = null;
  editBlogPreviewUrl: string | null = null;

  ngOnInit(): void {
    this.getBlogs();
    this.getCategories();
  }

  ngOnDestroy(): void {
    this.revokePreview('newCoverPreviewUrl');
    this.revokePreview('newBlogPreviewUrl');
    this.revokePreview('editCoverPreviewUrl');
    this.revokePreview('editBlogPreviewUrl');
  }

  get filteredBlogs(): BlogDto[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.blogs ?? [];

    return (this.blogs ?? []).filter((item: any) => {
      const haystack = [
        item?.title,
        item?.description,
        item?.category?.categoryName,
        item?.user?.fullName,
        item?.user?.userName
      ].filter(Boolean).join(' ').toLowerCase();

      return haystack.includes(term);
    });
  }

  get totalBlogs(): number {
    return this.blogs?.length ?? 0;
  }

  get categoriesInUse(): number {
    return new Set((this.blogs ?? []).map((x: any) => x?.category?.categoryName).filter(Boolean)).size;
  }

  get latestUpdatedLabel(): string {
    const dates = (this.blogs ?? [])
      .map((x: any) => new Date(x?.updatedAt ?? x?.createdAt ?? 0).getTime())
      .filter((x: number) => Number.isFinite(x) && x > 0);

    if (!dates.length) return 'No updates yet';
    return new Date(Math.max(...dates)).toLocaleDateString();
  }

  getBlogs(){
    this.blogService.getAll().subscribe({
      next: result => this.blogs = result ?? [],
      error: err => {
        console.error(err);
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? 'An Error Occured!')
      }
    })
  }

  onNewCoverSelected(e: any){
    this.newCoverFile = e?.target?.files?.[0] ?? null;
    this.setPreview('newCoverPreviewUrl', this.newCoverFile);
  }

  onNewBlogSelected(e: any){
    this.newBlogFile = e?.target?.files?.[0] ?? null;
    this.setPreview('newBlogPreviewUrl', this.newBlogFile);
  }

  onEditCoverSelected(e: any){
    this.editCoverFile = e?.target?.files?.[0] ?? null;
    this.setPreview('editCoverPreviewUrl', this.editCoverFile, this.editBlog?.coverImage ?? null);
  }

  onEditBlogSelected(e: any){
    this.editBlogFile = e?.target?.files?.[0] ?? null;
    this.setPreview('editBlogPreviewUrl', this.editBlogFile, this.editBlog?.blogImage ?? null);
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
        alertify.success(res?.message ?? 'Blog Created!');
        this.getBlogs();
      },
      error : result => {
        alertify.error(result?.error?.message ?? result?.error?.Message ?? result?.message ?? 'An Error Occured!');
        this.errors= result.error?.errors;
      },
      complete: () => {
        this.errors= {};
        this.newBlog = new BlogDto();
        this.newCoverFile = null;
        this.newBlogFile = null;
        this.revokePreview('newCoverPreviewUrl');
        this.revokePreview('newBlogPreviewUrl');
      }
    })
  }

  getCategories(){
    this.categoryService.getCategories().subscribe({
      next: result => this.categories= result ?? [],
      error: err => console.error(err)
    })
  }

  onSelected(blog: any){
    this.errors= {};
    this.editBlog= { ...blog };
    this.originalEditBlog = { ...blog };
    this.editCoverFile = null;
    this.editBlogFile = null;
    this.setPreview('editCoverPreviewUrl', null, this.editBlog?.coverImage ?? null);
    this.setPreview('editBlogPreviewUrl', null, this.editBlog?.blogImage ?? null);
  }

  update(){
    const changes: any = { id: this.editBlog?.id };

    if (this.originalEditBlog) {
      if (this.editBlog?.title !== this.originalEditBlog?.title) changes.title = this.editBlog?.title;
      if (this.editBlog?.description !== this.originalEditBlog?.description) changes.description = this.editBlog?.description;
      if (this.editBlog?.categoryId !== this.originalEditBlog?.categoryId) changes.categoryId = this.editBlog?.categoryId;
    }

    this.blogService.update(changes, this.editCoverFile, this.editBlogFile).subscribe({
      next: (res: any) => {
        alertify.success(res?.message ?? 'Blog Updated!');
        this.getBlogs();
        this.errors = {};
      },
      error: err =>{
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? 'An Error Occured!');
        this.errors = err.error?.errors
      },
      complete: () => {
        this.editCoverFile = null;
        this.editBlogFile = null;
        this.originalEditBlog = { ...this.editBlog };
      }
    })
  }

  async delete(id: string){
    const isConfirmed = await this.swal.areYouSure();

    if(isConfirmed){
      this.blogService.delete(id).subscribe({
        next: (res: any) => {
          alertify.success(res?.message ?? 'Blog Deleted!');
          this.getBlogs();
        },
        error: err => {
          console.error(err);
          alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? 'An Error Occured!');
        }
      })
    }
  }

  private setPreview(
    key: 'newCoverPreviewUrl' | 'newBlogPreviewUrl' | 'editCoverPreviewUrl' | 'editBlogPreviewUrl',
    file: File | null,
    fallbackUrl: string | null = null
  ) {
    this.revokePreview(key);

    if (file) {
      (this as any)[key] = URL.createObjectURL(file);
      return;
    }

    (this as any)[key] = fallbackUrl;
  }

  private revokePreview(key: 'newCoverPreviewUrl' | 'newBlogPreviewUrl' | 'editCoverPreviewUrl' | 'editBlogPreviewUrl') {
    const value = (this as any)[key];
    if (typeof value === 'string' && value.startsWith('blob:')) {
      URL.revokeObjectURL(value);
    }
    (this as any)[key] = null;
  }
}
