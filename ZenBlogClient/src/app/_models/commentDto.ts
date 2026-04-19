
import { BlogDto } from "./blog";
import { SubCommentDto } from "./subCommentDto";

export class CommentDto{
  id?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  blogId?: string;
  blog?: BlogDto;
  body?: string;
  commentDate?: string;
  createdAt?: string;
  updatedAt?: string;
  commenterImageUrl?: string;
  subComments: SubCommentDto[] = [];
}
