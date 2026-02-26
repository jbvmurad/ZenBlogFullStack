import { Component, OnInit } from '@angular/core';
import { ContactInfoDto } from '../../_models/contactInfoDto';
import { ContactInfoService } from '../../_services/contact-info-service';
import { AuthService } from '../../_services/auth-service';
import { SweetalertService } from '../../_services/sweetalert-service';
declare const alertify :any;

@Component({
  selector: 'app-contact-info',
  standalone: false,
  templateUrl: './contact-info.html',
  styleUrl: './contact-info.css'
})
export class ContactInfo implements OnInit {
contactInfos:ContactInfoDto[];
newContactInfo: ContactInfoDto= new ContactInfoDto();
editContactInfo:any ={};
errors:any= [];

ngOnInit(): void {

  this.getcontactInfos();

}


constructor(private contactInfoService : ContactInfoService,
            private authService: AuthService,
            private swal : SweetalertService
){

}

getcontactInfos(){
 this.contactInfoService.getAll().subscribe({
    next: result => this.contactInfos= result,
    error: err => {
      console.error(err);
      alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
    }
  })
}

create(){
  this.errors= {};
  this.contactInfoService.create(this.newContactInfo).subscribe({
    next: (res: any) => {
      alertify.success(res?.message ?? "Contact Info Created!");
      this.getcontactInfos();
      this.errors = {};
    },
    error : result => {
      alertify.error(result?.error?.message ?? result?.error?.Message ?? result?.message ?? "An Error Occured!");
    console.log(result.error?.errors);
      this.errors= result.error?.errors;


    },
    complete: () => { this.errors= {};
     }
  })
}




onSelected(model){
  this.errors= {};
this.editContactInfo= model;
}


update(){
  this.contactInfoService.update(this.editContactInfo).subscribe({
   next: (res: any) => {
    alertify.success(res?.message ?? "Contact Info Updated!");
    this.getcontactInfos();
    this.errors= {};
   },
   error: err =>{ alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!");
    this.errors = err.error?.errors
   }
  })
}

async delete(id){

const isConfirmed = await this.swal.areYouSure();

if(isConfirmed){

this.contactInfoService.delete(id).subscribe({
    next: (res: any) => {
      alertify.success(res?.message ?? "Contact Info Deleted!");
      this.getcontactInfos();
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
