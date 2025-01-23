import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { CommonModule, NgClass } from '@angular/common';
import Swal from 'sweetalert2'
import { UserService } from '../../Services/User/user.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, RouterLink, NgClass, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  constructor(private userService: UserService, private router: Router, private toastr : ToastrService) {}

  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required])
  });

  logFormData: any;
  loggedAgent: any;

  forVerifying(): void {
    debugger
    this.logFormData = this.loginForm.value;

    this.userService.loginUser(this.logFormData).subscribe({
      next: (value: any) => {
        this.loggedAgent = value;

        // sessionStorage.setItem('email', this.loggedAgent.email);
        // sessionStorage.setItem('role', this.loggedAgent.roleName);
        sessionStorage.setItem('token', this.loggedAgent.token);
        console.log(this.loggedAgent.token);
        // const tokenInfo = jwtDecode(this.loggedAgent.token);
        const helper = new JwtHelperService();

        const decodedToken = helper.decodeToken(this.loggedAgent.token);
        sessionStorage.setItem("roleName", decodedToken.roleName)
        sessionStorage.setItem("apiKey",decodedToken.apiKey)
        sessionStorage.setItem("userId",decodedToken.userId)
        sessionStorage.setItem("email",decodedToken.email)
        
        
        
        


        // Swal.fire({
        //   icon: "success",
        //   timer: 1500 ,
        //   showConfirmButton:false
        // });
        this.toastr.success("Logged in!!")
        this.router.navigateByUrl('/layout/dashboard');
      },
      error: (value: any) => {
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: "Incorrect username or password",
          showConfirmButton:false
        });
        this.resetForm();
      }
    });
  }

  resetForm(): void {
    this.loginForm.reset();
  }
}
