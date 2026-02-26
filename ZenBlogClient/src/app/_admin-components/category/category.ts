import { SweetalertService } from '../../_services/sweetalert-service';
import { Component } from '@angular/core';
import { CategoryService } from '../../_services/category-service';
import { CategoryDto } from '../../_models/category';
declare const alertify :any;

@Component({
  selector: 'admin-category',
  standalone: false,
  templateUrl: './category.html',
  styleUrl: './category.css'
})
export class Category {

constructor(private categoryService : CategoryService,
            private swal: SweetalertService
){

  this.getCategories();
}

categories: CategoryDto[];
newCategory: CategoryDto = new CategoryDto();
editCategory:any = {};
errors: any=[];


getCategories(){
  this.categoryService.getCategories().subscribe({
    next: result=> this.categories= result,
    error: err=> console.error(err)
  }
    )
  };

  createCategory(){
      this.categoryService.create(this.newCategory).subscribe({
        next: (res: any) => {
          alertify.success(res?.message ?? "Category Created!");
          this.errors = [];
          this.newCategory = new CategoryDto();
          this.getCategories();
        },
        error: err =>{
            alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!");
            if(err.status===400){
              console.log(err.error?.errors)
              this.errors= err.error?.errors
            } else {
              console.error(err);
            }
        }
      })
  };

async delete(id){

const isConfirmed = await this.swal.areYouSure();


if(isConfirmed){
this.categoryService.delete(id).subscribe({
  next: (res: any) => {
    alertify.success(res?.message ?? "Category Deleted!");
    this.getCategories();
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

update(){
this.categoryService.update(this.editCategory).subscribe({
  next: (res: any) => {
    alertify.success(res?.message ?? "Category Updated!");
    this.errors = [];
    this.getCategories();
  },
  error: err => {
    alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!");
    if(err.status===400){
              console.log(err.error)
              this.errors= err.error?.errors
            } else {
              console.error(err);
            }
  }

})

};


onSelected(model:CategoryDto){
  this.editCategory= model;
}



}




