export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    id: string;
    username: string;
    accessToken: string;
    refreshToken: string;
}

export interface LogoutRequest {
    tokenString: string; 
}