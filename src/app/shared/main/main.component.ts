import { Component, ViewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { SidenavComponent } from '../sidenav/sidenav.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'phds-main',
  standalone: true,
  imports: [
    CommonModule,
    HeaderComponent,
    RouterOutlet,
    SidenavComponent
  ],
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent {
  @ViewChild(SidenavComponent) sidenav!: SidenavComponent;

  isSidenavActive() {
    return this.sidenav && this.sidenav.active && this.sidenav.active.sub;
  }
}
