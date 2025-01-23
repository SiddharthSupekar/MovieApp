import { Component, NgModule } from '@angular/core';
import { MovieService } from '../../Services/Movie/movie.service';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormControl, FormGroup, FormsModule, NgModel, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import * as bootstrap from 'bootstrap'
import Swal from 'sweetalert2';
import { PhoneDirective } from '../../Directives/PhoneDirective/phone.directive';
import { Subject, Subscription } from 'rxjs';
import {debounceTime, distinctUntilChanged } from 'rxjs/operators'
import {ToastrService} from 'ngx-toastr'

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PhoneDirective, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

  constructor(private movieService : MovieService, private toastr : ToastrService){}

  allMovies:any;
  filteredMovies:any;
  selectedFile: File | null = null;
  image:any
  currentMovieImage:any;
  currentMovieId:any;
  currentDate = new Date();
  currentYear = this.currentDate.getFullYear()
  searchInput:string = '';
  private searchSubject = new Subject<string>();
  private searchSubscription!: Subscription;

  releaseDateValidator:ValidatorFn = (formGroup:AbstractControl)=>{
      const releaseDate = formGroup.get("releaseDate")?.value;
      return (releaseDate<= this.currentYear && releaseDate>=1900) ? null : { invalid: true };
    };

    movieForm:FormGroup=new FormGroup({
      movieName : new FormControl('',Validators.required),
      releaseDate : new FormControl('',[Validators.required,Validators.maxLength(4),Validators.max(this.currentYear),Validators.min(1900)]),
      genre: new FormControl('',Validators.required),
  
    }, this.releaseDateValidator
  )

  ngOnInit():void{
    this.getMovies();
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(searchInput => {
      const apiKey = sessionStorage.getItem("apiKey") || '';
      

      if (!searchInput.trim()) {
        this.getMovies();
        return;
      }

      this.movieService.getSearchMovies(searchInput, apiKey).subscribe({
        next: (value: any) => {
          this.filteredMovies = value.movies;
        }
      });
    });
  }

  getMovies(){
    this.movieService.getMovies().subscribe({
      next:(value:any)=>{
        this.filteredMovies = value;
        console.log(this.allMovies);
        
      }
    })
  }

  isAdmin():boolean{
    const role = sessionStorage.getItem("roleName")
    if(role === 'Admin'){
      return true
    }
    else{
      return false
    }
  }

  searchItem:FormGroup = new FormGroup({
    searchTitle : new FormControl('')
  })
  onSearch(){
    debugger
    const apiKey = sessionStorage.getItem("apiKey") || ''
    const searchInput = this.searchInput
    this.movieService.getSearchMovies(searchInput,apiKey).subscribe({
      next:(value:any)=>{
        this.filteredMovies = value
      }
    })
  }
  
  onSearchChange(event:string){
    debugger
    // const apiKey = sessionStorage.getItem("apiKey") || ''
    // const searchInput = event;
    // this.movieService.getSearchMovies(searchInput,apiKey).subscribe({
    //   next:(value:any)=>{
    //     this.filteredMovies = value
    //   }
    // })
    this.searchSubject.next(event);
  }
  ngOnDestroy() {
    this.searchSubscription.unsubscribe();
  }
  addMovie(){
    if(this.movieForm.invalid){
      this.movieForm.markAllAsTouched();
    }
    if(this.movieForm.valid){
      debugger
      const formData = new FormData();
      Object.keys(this.movieForm.controls).forEach((key) => {
        formData.append(key, this.movieForm.get(key)?.value as string);
      });

      if(this.selectedFile){
        formData.append('posterImage', this.selectedFile, this.selectedFile.name)
      }
      console.log(formData);
      

      this.movieService.addMovie(formData).subscribe({
        next : (value:any)=>{
          debugger
          if(value.status === 200){
            // Swal.fire({
            //   icon: "success",
            //   title: "Profile updated",
            //   showConfirmButton: true,
            //   timer: 1000
            // });
            this.toastr.success("Movie Added")
          }
          else{
            this.toastr.error("Could not add the movie")
          }
          this.getMovies();
          this.closeMovieUpdateModal()
        },
        error :(error : any)=>{
          Swal.fire({
            icon: "error",
            title: "Error!!"
          });
          console.log(error);
          this.closeMovieUpdateModal()
          
        }
      });
  
    }
  }
  onUpdate(movie:any){
    this.openMovieUpdateModal();
    this.movieForm.get('movieName')?.setValue(movie.movieName);
    this.movieForm.get('releaseDate')?.setValue(movie.releaseDate);
    this.movieForm.get('genre')?.setValue(movie.genre);
    this.currentMovieImage = movie.posterImage
    this.currentMovieId = movie.movieId
  }
  updateMovie(){
    debugger
    if(this.movieForm.invalid){
      this.movieForm.markAllAsTouched();
    }
    if(this.movieForm.valid){
      debugger
      const formData = new FormData();
      Object.keys(this.movieForm.controls).forEach((key) => {
        formData.append(key, this.movieForm.get(key)?.value as string);
      });

      if(this.selectedFile){
        formData.append('posterImage', this.selectedFile, this.selectedFile.name)
      }
      console.log(formData);
      

      this.movieService.updateMovie(this.currentMovieId,formData).subscribe({
        next : (value:any)=>{
          debugger
          // Swal.fire({
          //   icon: "success",
          //   title: "Profile updated",
          //   showConfirmButton: true,
          //   timer: 1000
          // });
          if(value.status === 200){
            this.toastr.success("Details updated successfully!")
          }
          else{
            this.toastr.error("Could not update the movie")
          }
          this.getMovies();
          this.closeMovieUpdateModal()
        },
        error :(error : any)=>{
          Swal.fire({
            icon: "error",
            title: "Error!!",
            text: "Either null value or the image profile picture is not selected",
          });
          console.log(error);
          this.closeMovieUpdateModal()
          
        }
      });
  
    }
  }
  onDelete(movieId:number){
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!"
    }).then((result) => {
      if (result.isConfirmed) {
        this.movieService.deleteMovie(movieId).subscribe({
          next:(value:any)=>{
            // Swal.fire({
            //   icon: "success",
            //   title: "Product deleted successfully",
            //   showConfirmButton: false,
            //   timer: 1200
            // });
            if(value.status === 200){
              this.toastr.success("Deleted successfully")
            }
            else{
              this.toastr.error("Could not delete")
            }
            this.getMovies();
            this.closeMovieUpdateModal()
          }
        })
        // Swal.fire({
        //   title: "Deleted!",
        //   text: "Your file has been deleted.",
        //   icon: "success"
        // });
      }
    });
  }

  onFileSelected(event: any): void {
    debugger
    this.selectedFile = event.target.files[0];
    const imagefromhtml = event.target as HTMLInputElement
    if(imagefromhtml.files && imagefromhtml.files[0]){
      const file = imagefromhtml.files[0]
      var reader = new FileReader();
    reader.onload=()=>{
      this.image = reader.result
    };
    reader.readAsDataURL(file)
    }
  }
  closeMovieUpdateModal() {
    const otpModalElement = document.getElementById('updateModal');
    if (otpModalElement) {
      const profileModalInstance = bootstrap.Modal.getInstance(otpModalElement);
      profileModalInstance?.hide();
    }
  }
  
  openMovieUpdateModal() {
    const modalElement = document.getElementById('updateModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }
  closeMovieAddModal() {
    const otpModalElement = document.getElementById('addModal');
    if (otpModalElement) {
      const profileModalInstance = bootstrap.Modal.getInstance(otpModalElement);
      profileModalInstance?.hide();
    }
  }
  
  openMovieAddModal() {
    const modalElement = document.getElementById('addModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }
  isFieldInvalid(fieldName: string): boolean {
    const control = this.movieForm.get(fieldName);
    return control ? control.invalid && (control.dirty || control.touched) : false;
  }

}
