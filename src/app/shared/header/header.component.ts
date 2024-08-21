import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
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

  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.authService.getUserInfo().subscribe(
      (res) => {
        this.user = res;
      }
    );

    this.router.events.subscribe(() => {
      this.isHomeRoute = this.router.url === '/';
    });
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
