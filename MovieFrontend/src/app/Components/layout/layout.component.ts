import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

import { CommonModule } from '@angular/common';


@Component({
    selector: 'app-layout',
    standalone:true,
    imports: [CommonModule, RouterLink, RouterOutlet],
    templateUrl: './layout.component.html',
    styleUrl: './layout.component.css'
})
export class LayoutComponent {

  constructor(private router : Router){}




  ngOnInit():void{
  };
  



  logout(){
    sessionStorage.clear();
    this.router.navigateByUrl('/login');
    
  }

}


  

