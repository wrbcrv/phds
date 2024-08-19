import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'phds-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  user: any;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.getUserInfo().subscribe(
      (res) => {
        this.user = res;
      },
      (err) => {

      }
    );
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
