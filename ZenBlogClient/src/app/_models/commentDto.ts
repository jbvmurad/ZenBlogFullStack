
import { BlogDto } from "./blog";
import { SubCommentDto } from "./subCommentDto";
import { UserDto } from "./userDto";

export class CommentDto{
  id;
  firstName;
  lastName;
  email;
  blogId;
  blog:BlogDto;
  body;
  commentDate;
  createdAt;
  updatedAt;
  subComments: SubCommentDto[];
}
