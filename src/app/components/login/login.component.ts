import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  username: string = '';
  password: string = '';

  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onSubmit(): void {
    this.authService.login(this.username, this.password).subscribe(
      (res) => {
        this.router.navigateByUrl('/ticket-list')
      },
      (err) => {
        this.errorMessage = err.error; 
        console.log(err);
        setTimeout(() => {
          this.errorMessage = null;
        }, 5000);
      }
    );
  }
}
