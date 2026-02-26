import { Component } from '@angular/core';
import { CommentService } from '../../_services/comment-service';
import { SweetalertService } from '../../_services/sweetalert-service';
import { CommentDto } from '../../_models/commentDto';
declare const alertify:any;

@Component({
  selector: 'app-comment',
  standalone: false,
  templateUrl: './comment.html',
  styleUrl: './comment.css'
})
export class Comment {

constructor(private commentService: CommentService,
            private swal: SweetalertService
){
  this.getcomments();
}


comments: CommentDto[];
newComment: CommentDto = new CommentDto();
editComment:any = {};
errors: any=[];


getcomments(){
  this.commentService.getAll().subscribe({
    next: (result: any) => {
      const list: any[] = Array.isArray(result) ? result : [];
      // Some older/test records can exist with missing fields; don't render empty rows in the table.
      this.comments = list.filter((x: any) => {
        const firstName = (x?.firstName?.trim?.() ?? '');
        const lastName  = (x?.lastName?.trim?.() ?? '');
        const email     = (x?.email?.trim?.() ?? '');
        const body      = (x?.body?.trim?.() ?? '');
        return firstName.length > 0 && lastName.length > 0 && email.length > 0 && body.length > 0;
      });
    },
    error: err=> console.error(err)
  }
    )
  };



async delete(id){

const isConfirmed = await this.swal.areYouSure();


if(isConfirmed){
this.commentService.delete(id).subscribe({
  next: (res: any) => {
    alertify.success(res?.message ?? "Comment Deleted!");
    this.getcomments();
  },
  error: err => {
    console.error(err);
    alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
  }
})

}
else{
  console.log("Delete Reverted")
}



};





}
