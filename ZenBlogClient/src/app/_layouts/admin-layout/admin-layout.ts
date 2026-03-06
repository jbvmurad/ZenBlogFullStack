import { Component } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { AccessControlService } from '../../_services/access-control-service';

@Component({
  selector: 'admin-layout',
  standalone: false,
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css'
})
export class AdminLayout {

constructor(private authService: AuthService, private access: AccessControlService){}



getUserName(){
 let decodedToken= this.authService.decodeToken();
 return decodedToken.name
}

logout(){

  this.access.clear();
  this.authService.logout();
}


}
