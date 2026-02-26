import { Component, OnInit } from '@angular/core';
import { SocialDto } from '../../_models/socialDto';
import { SocialService } from '../../_services/social-service';
import { AuthService } from '../../_services/auth-service';
import { SweetalertService } from '../../_services/sweetalert-service';
declare const alertify : any;

@Component({
  selector: 'app-social',
  standalone: false,
  templateUrl: './social.html',
  styleUrl: './social.css'
})
export class Social implements OnInit {
socials:SocialDto[];
newSocial: SocialDto= new SocialDto();
editSocial:any ={};
errors:any= [];

// Store-style file pickers (same UX as Blog admin)
newIconFile: File | null = null;
editIconFile: File | null = null;

ngOnInit(): void {

  this.getSocials();

}


constructor(private socialService : SocialService,
            private authService: AuthService,
            private swal : SweetalertService
){

}

getSocials(){
 this.socialService.getAll().subscribe({
    next: result => this.socials= result,
    error: err => {
      console.error(err);
      alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
    }
  })
}

create(){
  this.errors= {};
  if (!this.newIconFile) {
    alertify.error('Please choose an Icon file.');
    return;
  }

  this.socialService.createWithMedia(this.newSocial, this.newIconFile).subscribe({
    next: (res: any) => {
      alertify.success(res?.message ?? "Social Created!");
      this.getSocials();
      this.errors= {};
      this.newIconFile = null;
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
this.editSocial= model;
this.editIconFile = null;
}


update(){
  this.socialService.updateWithMedia(this.editSocial, this.editIconFile).subscribe({
   next: (res: any) => {
    alertify.success(res?.message ?? "Social Updated!");
    this.getSocials();
    this.errors= {};
    this.editIconFile = null;
   },
   error: err =>{ alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!");
    this.errors = err.error?.errors
   }
  })
}

onNewIconSelected(e: any){
  this.newIconFile = e?.target?.files?.[0] ?? null;
}

onEditIconSelected(e: any){
  this.editIconFile = e?.target?.files?.[0] ?? null;
}

isIconClass(icon: any): boolean {
  if (typeof icon !== 'string' || !icon) return false;
  // Heuristic: old UI stored FontAwesome class names (no slashes, no file ext)
  const looksLikePath = icon.includes('/') || icon.includes('\\') || /\.(png|jpe?g|webp|svg)$/i.test(icon);
  return !looksLikePath;
}

async delete(id){

const isConfirmed = await this.swal.areYouSure();

if(isConfirmed){

this.socialService.delete(id).subscribe({
    next: (res: any) => {
      alertify.success(res?.message ?? "Social Deleted!");
      this.getSocials();
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
