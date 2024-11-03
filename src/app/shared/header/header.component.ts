import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { DropdownService } from '../../services/dropdown.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'phds-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  user: any;
  isHomeRoute: boolean = false;
  notifications: any[] = [];

  constructor(
    private authService: AuthService,
    private router: Router,
    private userService: UserService,
    private dropdownService: DropdownService
  ) { }

  ngOnInit(): void {
    this.authService.user$.subscribe(
      (user) => {
        this.user = user;
        this.loadNotifications();
      }
    );

    this.router.events.subscribe(() => {
      this.isHomeRoute = this.router.url === '/';
    });

    this.authService.getUserInfo().subscribe();
  }

  loadNotifications(): void {
    this.authService.getNotifications().subscribe(
      (res) => {
        this.notifications = res;
      },
      (err) => {

      }
    );
  }

  deleteNotification(id: number): void {
    this.authService.deleteNotification(id).subscribe(
      (res) => {
        this.loadNotifications();
      }
    )
  }

  logout(): void {
    this.authService.logout().subscribe(
      (res) => {
        this.router.navigate(['/']);
      }
    );
  }

  toggleUserDropdown(): void {
    if (this.dropdownService.isOpen('userDropdown')) {
      this.dropdownService.closeDropdown();
    } else {
      this.dropdownService.setOpenDropdown('userDropdown');
    }
  }

  toggleNotificationsDropdown(): void {
    if (this.dropdownService.isOpen('notificationsDropdown')) {
      this.dropdownService.closeDropdown();
    } else {
      this.dropdownService.setOpenDropdown('notificationsDropdown');
    }
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    const target = event.target as HTMLElement;
    const clickedInsideUser = target.closest('.user-info');
    const clickedInsideNotifications = target.closest('.relative.cursor-pointer');

    if (!clickedInsideUser && this.dropdownService.isOpen('userDropdown')) {
      this.dropdownService.closeDropdown();
    }

    if (!clickedInsideNotifications && this.dropdownService.isOpen('notificationsDropdown')) {
      this.dropdownService.closeDropdown();
    }
  }

  isNotificationsDropdownOpen(): boolean {
    return this.dropdownService.isOpen('notificationsDropdown');
  }

  isUserDropdownOpen(): boolean {
    return this.dropdownService.isOpen('userDropdown');
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
