import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { AuthService } from './identity/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterModule, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'biography';
  isMobileMenuOpen = false;
  public isSignedIn: boolean = false;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.authService.onStateChanged().forEach((state) => {
      // alert("state changed")
      this.authService.isSignedIn().forEach((signedIn: boolean) => this.isSignedIn = signedIn).catch(() => this.isSignedIn = false);
    })
  }

  signOut() {
    if (this.isSignedIn) {
      this.authService.signOut().forEach(response => {
        if (response) {
          this.router.navigateByUrl('');
        }
      });
    }
  }

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }
}
