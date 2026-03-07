import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-home',
   standalone: true,
  imports: [CommonModule, MatIconModule],  // Import CommonModule for *ngIf to work
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
showDetails: boolean = false;

  toggleDetails(): void {
    this.showDetails = !this.showDetails;
  }

}
