import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
   standalone: true,
  imports: [CommonModule],  // Import CommonModule for *ngIf to work
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
showDetails: boolean = false;

  toggleDetails(): void {
    this.showDetails = !this.showDetails;
  }

}
