import { Component, OnInit } from '@angular/core';
import { MessageDto } from '../../_models/messageDto';
import { MessageService } from '../../_services/message-service';
import { AuthService } from '../../_services/auth-service';
import { SweetalertService } from '../../_services/sweetalert-service';
declare const alertify : any;

@Component({
  selector: 'app-message',
  standalone: false,
  templateUrl: './message.html',
  styleUrl: './message.css'
})
export class Message implements OnInit {

  messages:MessageDto[];
  unreadMessages: MessageDto[];
  readMessages: MessageDto[];
  newMessage: MessageDto= new MessageDto();
  editMessage:any ={};
  errors:any= [];

  ngOnInit(): void {

    this.getUnReadMessages();
    this.getReadMessages();


  }


  constructor(private messageService : MessageService,
              private authService: AuthService,
              private swal : SweetalertService
  ){

  }

  getmessages(){
   this.messageService.getAll().subscribe({
      next: result => this.messages= result,
      error: err => {
        console.error(err);
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
      }
    })
  }

  getUnReadMessages(){
   this.messageService.getUnreadMessages().subscribe({
      next: result => this.unreadMessages= result,
      error: err => {
        console.error(err);
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
      }
    })
  }

  getReadMessages(){
   this.messageService.getReadMessages().subscribe({
      next: result => this.readMessages= result,
      error: err => {
        console.error(err);
        alertify.error(err?.error?.message ??  err?.error?.Message ?? err?.message ?? "An Error Occured!")
      }
    })
  }


  create(){
    this.errors= {};
    this.messageService.create(this.newMessage).subscribe({
      next: (res: any) => {
        alertify.success(res?.message ?? "Message Created!");
        this.getReadMessages();
        this.getUnReadMessages();
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

  this.editMessage= model;

  this.editMessage.isRead = true;

  this.messageService.update(this.editMessage).subscribe({
    next: ()=> {this.getReadMessages();
      this.getUnReadMessages();
    },
    error: err => console.log(err.error)
  })
  }


  update(){
    this.messageService.update(this.editMessage).subscribe({
     next: (res: any) => {
      alertify.success(res?.message ?? "Message Updated!");
      this.getReadMessages();
      this.getUnReadMessages();
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

  this.messageService.delete(id).subscribe({
      next: (res: any) => {
        alertify.success(res?.message ?? "Message Deleted!");
        this.getReadMessages();
        this.getUnReadMessages();
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
