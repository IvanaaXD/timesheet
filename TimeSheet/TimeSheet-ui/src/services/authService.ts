import api from '../config/axios';
import { AuthResponse, LoginRequest, LogoutRequest } from '../types/auth';
import { getUserRole } from '../utils/authUtils';

const BASE_URL = '/api/Auth';

export const authService = {
    login: async (data: LoginRequest): Promise<AuthResponse> => {
        const response = await api.post<AuthResponse>(`${BASE_URL}/login`, data);
        
        if (response.data.accessToken) {
            localStorage.setItem('accessToken', response.data.accessToken);
            localStorage.setItem('refreshToken', response.data.refreshToken);
            
            const role = getUserRole(); 

            localStorage.setItem('user', JSON.stringify({ 
                id: response.data.id, 
                username: response.data.username,
                role: role 
            }));
        }
        
        return response.data;
    },

    logout: async (): Promise<void> => {
        const refreshToken = localStorage.getItem('refreshToken');
        
        if (refreshToken) {
            const data: LogoutRequest = { tokenString: refreshToken };
            try {
                await api.post(`${BASE_URL}/logout`, data);
            } catch (error) {
                console.error("Грешка при одјављивању на серверу", error);
            }
        }

        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        
        window.location.href = '/login';
    },

    // refreshToken: async (accessToken: string, refreshToken: string): Promise<AuthResponse> => {
    //     const response = await api.post<AuthResponse>(`${BASE_URL}/refresh`, { accessToken, refreshToken });
    //     return response.data;
    // }
};