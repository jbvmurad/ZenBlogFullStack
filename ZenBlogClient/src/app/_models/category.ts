import { BlogDto } from "../_models/blog";


export class CategoryDto{
  id;
  categoryName;
  blogs:BlogDto[];
}
