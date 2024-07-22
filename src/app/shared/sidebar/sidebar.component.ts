import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'phds-sidenav',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  drawerOpen: string | null = null;

  toggleDrawer(menuItem: string) {
    if (this.drawerOpen === menuItem) {
      this.drawerOpen = null;
    }

    if (this.drawerOpen !== menuItem) {
      this.drawerOpen = menuItem;
    }
  }
}
