import { HttpClient, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Subject, Observable, BehaviorSubject, catchError, map, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { UserInfo } from "./dto";


interface Account {
  username: string;
  password: string; 
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl = 'http://localhost:5199';

  constructor(private http: HttpClient) { }

  private _authStateChanged: Subject<boolean> = new BehaviorSubject<boolean>(false);
  private token = localStorage.getItem('token') || '';

  public onStateChanged() {
    return this._authStateChanged.asObservable();
  }

  // cookie-based login
  public signIn(username: string, password: string) {
    const response = this.http.post('/api/auth/login', {
      username: username,
      password: password
    }, {
      observe: 'response',
      responseType: 'text'
    })
    .pipe<boolean>(map((res: HttpResponse<string>) => {
      if (res.ok) {
        if (res.body) {
          const token = JSON.parse(res.body).token;
          localStorage.setItem('token', token);
          this.setUsername(username);
          this.token = token
        }

        this._authStateChanged.next(true);
      }
      return res.ok;
    }));

    return response;
  }

  // register new user
  public register(username: string, password: string) {
    return this.http.post('/api/auth/register', {
      username: username,
      password: password
    }, {
      observe: 'response',
      responseType: 'text'
    })
  }

  // sign out
  public signOut() {
    this._authStateChanged.next(false);
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    this.token = '';
    return of(true);

    // return this.http.post('/api/logout', {}, {
    //   withCredentials: true,
    //   observe: 'response',
    //   responseType: 'text'
    // }).pipe<boolean>(map((res: HttpResponse<string>) => {
    //   if (res.ok) {
    //     this._authStateChanged.next(false);
    //   }
    //   return res.ok;
    // }));    
  }

   // check if the user is authenticated. the endpoint is protected so 401 if not.
  public user() {
    // return this.http.get<UserInfo>('/api/userinfo', {
    //   withCredentials: true, headers: {
    //     'Authorization': `Bearer ${this.token}`
    //   }
    // }).pipe(
    return this.http.get<UserInfo>('/api/auth/userinfo').pipe(
      catchError((_: HttpErrorResponse, __: Observable<UserInfo>) => {
        return of({} as UserInfo);
      }));
  }

  // is signed in when the call completes without error and the user has an email
  public isSignedIn(): Observable<boolean> {
    if (!this.token) return of(false);
    
    return this.user().pipe(
      map((userInfo) => {
        const valid = !!(userInfo && userInfo.username && userInfo.username.length > 0);
        return valid;
      }),
      catchError((_) => {
        return of(false);
      }));
  }
  
  getUsername() {
    return localStorage.getItem('username')?.toString() || '';
  //  return this.user().pipe<string>(map(user => user.username));
  }

  setUsername(username: string) {
    localStorage.setItem('username', username);
  //  return this.user().pipe<string>(map(user => user.username));
  }

  isLoggedIn(): boolean {
    return localStorage.getItem('username') != null;
  }

  getAuthToken(): string {
    return localStorage.getItem('token') || '';
  }
}
