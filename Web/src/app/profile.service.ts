import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

export interface Profile {
  username: string;
  name: string;
  biography: string;
  avatar?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = '/api/profile';

  constructor(private http: HttpClient) {}

  getProfile(): Observable<Profile> {
    // return of({
    //   username: 'username',
    //   name: 'name',
    //   biography: 'biography'
    // })
    return this.http.get<Profile>(this.apiUrl);
  }

  updateProfile(profile: Partial<Profile>, avatarFile?: File): Observable<Profile> {
    const formData = new FormData();
    
    // Add profile data
    if (profile.username) formData.append('username', profile.username);
    if (profile.name) formData.append('name', profile.name);
    if (profile.biography) formData.append('biography', profile.biography);
    
    // Add avatar file if provided
    if (avatarFile) {
      formData.append('avatar', avatarFile);
    }

    // return of({
    //   username: 'username',
    //   name: 'name',
    //   biography: 'biography'
    // })

    return this.http.put<Profile>(this.apiUrl, formData);
  }
}