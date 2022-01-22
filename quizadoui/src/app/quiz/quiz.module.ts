import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuizComponent } from './Components/quiz/quiz.component';
import { FinishPageComponent } from './Components/finish-page/finish-page.component';



@NgModule({
  declarations: [QuizComponent, FinishPageComponent],
  imports: [
    CommonModule
  ]
})
export class QuizModule { }
