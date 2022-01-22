import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './Component/login/login.component';
import { RegisterComponent } from './Component/register/register.component';
import { LandingPageComponent } from './Component/landing-page/landing-page.component';



@NgModule({
  declarations: [LoginComponent, RegisterComponent, LandingPageComponent],
  imports: [
    CommonModule
  ]
})
export class LandingModule { }
