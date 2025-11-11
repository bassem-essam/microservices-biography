// login and register
export interface UserDto {
    email: string;
    password: string;
}

// manage/info
export interface UserInfo {
    username: string;
}

export interface ErrorResponse {
  errors: { [key: string]: string[] }
}