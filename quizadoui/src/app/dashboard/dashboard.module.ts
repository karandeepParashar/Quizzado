import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { LeaderBoardComponent } from './Components/leader-board/leader-board.component';



@NgModule({
  declarations: [DashboardComponent, LeaderBoardComponent],
  imports: [
    CommonModule
  ]
})
export class DashboardModule { }
