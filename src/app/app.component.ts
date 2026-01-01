import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppmenuComponent } from './Component/appmenu/appmenu.component';

@Component({
  selector: 'app-root',
  imports: [CommonModule,AppmenuComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'store-app';
}
