import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'phds-header',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  user: any;
  isHomeRoute: boolean = false;
  isDropdownOpen: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.authService.user$.subscribe(
      (user) => {
        this.user = user;
      }
    );

    this.router.events.subscribe(() => {
      this.isHomeRoute = this.router.url === '/';
    });
  }

  logout(): void {
    this.authService.logout().subscribe(
      (res) => {
        this.router.navigate(['/']);
      }
    );
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    const target = event.target as HTMLElement;
    const clickedInside = target.closest('.user-info');
    if (!clickedInside) {
      this.isDropdownOpen = false;
    }
  }

  getInitials(fullName: string): string {
    if (!fullName) return '';
    const names = fullName.split(' ');
    return names.length > 1 ? names[0][0] + names[1][0] : names[0][0];
  }

  getNameAndSurname(fullName: string): string {
    if (!fullName) return '';
    const names = fullName.split(' ');
    return names.length > 1 ? `${names[0]} ${names[1]}` : names[0];
  }
}
