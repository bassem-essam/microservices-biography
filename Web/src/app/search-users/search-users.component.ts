import { Component } from '@angular/core';
import { UserService } from '../user.service';
import { User } from '../user';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserListingComponent } from "../user-listing/user-listing.component";

@Component({
  selector: 'app-search-users',
  imports: [CommonModule, FormsModule, UserListingComponent],
  templateUrl: './search-users.component.html',
  styles: ``
})
export class SearchUsersComponent {
  searchQuery = '';
  searchResults: User[] = [];

  constructor(private userService: UserService) { }

  searchUsers(): void {
    if (this.searchQuery.trim()) {
      this.userService.searchUsers(this.searchQuery).subscribe(users =>
        this.searchResults = users
      );
    } else {
      this.searchResults = [];
    }
  }
}
