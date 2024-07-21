import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'phds-sidenav',
  standalone: true,
  imports: [
    RouterModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {

}
