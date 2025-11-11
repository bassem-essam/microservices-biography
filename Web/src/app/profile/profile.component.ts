import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService, Profile } from '../profile.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  profile: Profile | null = null;
  formData: Partial<Profile> = {};
  selectedFile: File | null = null;
  loading = false;
  successMessage = '';
  errorMessage = '';
  uploadedImageUrl: string | null = null;
  showMessage = false;

  constructor(private profileService: ProfileService) {}

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.formData = { ...profile };
      },
      error: (err) => {
        this.errorMessage = 'Failed to load profile';
        console.error(err);
      }
    });
  }

  onFileSelect(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedFile = input.files[0];
      
      const reader = new FileReader();
      reader.onload = () => {
        this.uploadedImageUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  onSubmit() {
    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';
    this.showMessage = true;
    setTimeout(() => {
      this.showMessage = false;
    }, 3000)

    this.profileService.updateProfile(this.formData, this.selectedFile || undefined).subscribe({
      next: (updatedProfile) => {
        this.profile = updatedProfile;
        this.formData = { ...updatedProfile };
        this.selectedFile = null;
        this.successMessage = 'Profile updated successfully!';
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to update profile';
        this.loading = false;
        console.error(err);
      }
    });
  }
}