import { Component, inject } from '@angular/core';
import { UserService } from '../user.service';
import { UserListingComponent } from '../user-listing/user-listing.component';
import { CommonModule } from '@angular/common';
import { User } from '../user';

@Component({
  selector: 'app-top-users',
  imports: [UserListingComponent, CommonModule],
  templateUrl: './top-users.component.html',
  styles: ``
})
export class TopUsersComponent {
  userService: UserService = inject(UserService)
  error: string = '';
  topUsers: User[] = [];

  constructor() {
    this.userService.getTopUsers().subscribe({
      next: users => this.topUsers = users,
      error: err => { this.error = err.message; console.error(err) }
    })
  }
}
