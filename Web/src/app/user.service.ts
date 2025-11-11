import { Injectable } from '@angular/core';
import { User } from './user';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private GET_USER_URL = '/api/user';
  private SEARCH_USERS_URL = '/api/search-users';
  private TOP_USERS_URL = '/api/top-users';
  private TOP_USERS_LIMIT = 5;

  // users: User[] = [
  //   { username: 'johndoe', name: 'John Doe', biography: 'This is my bio.', avatar: '' },
  //   { username: 'janedoe', name: 'Jane Doe', biography: 'This is my bio.', avatar: '' },
  //   { username: 'bobsmith', name: 'Bob Smith', biography: 'This is my bio.', avatar: '' },
  //   { username: 'sallysmith', name: 'Sally Smith', biography: 'This is my bio.', avatar: '' }
  // ]

  constructor(private http: HttpClient) { }

  getUser(username: string): Observable<User> {
    return this.http.get<User>(this.GET_USER_URL + '/' + username);
  }

  getTopUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.TOP_USERS_URL + '?limit=' + this.TOP_USERS_LIMIT);
  }


  searchUsers(query: string): Observable<User[]>  {
    return this.http.get<User[]>(this.SEARCH_USERS_URL + '?searchTerm=' + encodeURIComponent(query));
  }
}
