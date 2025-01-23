import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl, ValidatorFn, AbstractControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule, formatDate } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { customEmailValidator } from './emailValidator';
import { UserService } from '../../Services/User/user.service';
// import { customEmailValidator } from './emailValidator';
import Swal from 'sweetalert2'
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  private router = inject(Router);

  selectedFile: File | null = null;
  isSubmitting = false; 
  submissionMessage: string | null = null; 
  countryData: any;
  filteredStateData: any;
  roles: any;
  image:Blob | null = null
  todayDate:any = formatDate(new Date(), "yyyy-MM-dd", "en")


  constructor(
    private fb: FormBuilder, 
    private userService : UserService,
    private toastr : ToastrService
  ) {}
  passwordMatchValidator:ValidatorFn = (formGroup:AbstractControl)=>{
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('cnfPassword')?.value;
    return password === confirmPassword ? null : { invalid: true };
  };
  registerForm = new FormGroup({
    firstName: new FormControl('', [Validators.required,Validators.maxLength(20)]),
    lastName: new FormControl('',  [Validators.required,Validators.maxLength(20)]),
    email: new FormControl('', [Validators.required, customEmailValidator()]),
    password : new FormControl('',Validators.required),
    cnfPassword : new FormControl('',Validators.required),
    roleId: new FormControl('', Validators.required),
  },this.passwordMatchValidator
);

  ngOnInit(): void {
   
    this.getRoles();
  }


  

  onSubmit(): void {

    if (this.registerForm.invalid) {
      console.log(this.registerForm.value);
      
      this.registerForm.markAllAsTouched();
    }

    if(this.registerForm.valid){
      this.isSubmitting = true;

    const formData = this.registerForm.value

    console.log('FormData to be submitted:', formData);

    this.userService.addUser(formData).subscribe({
      next: (value: any) => {

        this.toastr.success("Account created!!")
        console.log(value);
        const cartId = value;
        sessionStorage.setItem("cartId", cartId)
        this.router.navigateByUrl('/login');
      },
      error: (error: any) => {
        console.error(error);
        Swal.fire({
          icon: "error",
          title: "Error",
          text:"Entered email address already exists",
          showConfirmButton: false,
          timer: 1000
        });
        this.submissionMessage = 'An error occurred while submitting the form.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
    }
  }

  getRoles(): void {
    this.userService.getRoles().subscribe({
      next: (value: any) => {
        this.roles = value;

      },
      error: (error: any) => {
        console.error(error);
      }
    });
  }
  isFieldInvalid(fieldName: string): boolean {
    const control = this.registerForm.get(fieldName);
    return control ? control.invalid && (control.dirty || control.touched) : false;
  }

  
}
