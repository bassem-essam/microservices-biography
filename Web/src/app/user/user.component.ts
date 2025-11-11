import { Component, inject } from '@angular/core';
import { User } from '../user';
import { UserService } from '../user.service';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user',
  imports: [CommonModule],
  templateUrl: './user.component.html',
  styles: ``
})
export class UserComponent {
  userService: UserService = inject(UserService);
  route: ActivatedRoute = inject(ActivatedRoute);
  title: Title = inject(Title);
  loading: boolean = true;
  constructor() {
    const username = this.route.snapshot.paramMap.get('username');
    if (username) {
      this.userService.getUser(username).subscribe({
        next: user => {
          this.user = user;
          this.title.setTitle("Biography of " + this.user.name);
          this.loading = false;
        },
        error: err => {
          console.error(err);
          this.loading = false;
        },
      });
      // this.user = this.userService.getUser(username);
    } else {
      this.loading = false;
      // this.user = this.userService.getTopUsers()[0];
    }

  }

  user: User = {
    username: 'johndoe',
    name: 'John Doe',
    biography: 'Lorem ipsum dolor sit amet consectetur, adipisicing elit. Sit, nisi beatae. Quibusdam tempora alias, consectetur quo in rem non, culpa eius corporis nostrum numquam ad esse rerum, porro error explicabo.',
    avatar: '',
    visitCount: 1
  }
}
