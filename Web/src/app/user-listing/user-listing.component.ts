import { Component, inject, Input } from '@angular/core';
import { User } from '../user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-listing',
  imports: [],
  templateUrl: './user-listing.component.html',
  styles: ``
})
export class UserListingComponent {
  router: Router = inject(Router);
  @Input() user: User = {
    username: '',
    name: '',
    biography: '',
    avatar: '',
    visitCount: 0
  };

  navigateToUser(username: string) {
    this.router.navigate(['/user', username]);
  }
}
