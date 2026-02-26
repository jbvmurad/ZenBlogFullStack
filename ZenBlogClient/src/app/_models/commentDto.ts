
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
  // Server BaseEntity fields are serialized as camelCase by ASP.NET (CreatedAt -> createdAt, UpdatedAt -> updatedAt)
  // These are used in admin listing screens.
  createdAt;
  updatedAt;
  subComments: SubCommentDto[];
}
