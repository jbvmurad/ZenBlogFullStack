import { Component } from '@angular/core';
import { BlogService } from '../../_services/blog-service';
import { ActivatedRoute } from '@angular/router';
import { BlogDto } from '../../_models/blog';
import { CommentDto } from '../../_models/commentDto';

@Component({
  selector: 'app-blogdetails',
  standalone: false,
  templateUrl: './blogdetails.html',
  styleUrl: './blogdetails.css'
})
export class Blogdetails {
blog:BlogDto;
latestBlogs: BlogDto[];
newComment : CommentDto = new CommentDto();


constructor(private blogService: BlogService,
            private route: ActivatedRoute
){
  this.getBlogById();
  this.getLatestBlogs();
}



getBlogById(){
    this.blogService.getBlogById(this.route.snapshot.params["id"]).subscribe({
      next: result => this.blog= result
    })
}

getLatestBlogs(){
  this.blogService.getLatest5Blogs().subscribe({
    next: result => this.latestBlogs= result
  })
}

postComment(){
  this.newComment.blogId = this.route.snapshot.params["id"];

}

}
