import { jwtDecode } from 'jwt-decode';

export const getUserRole = (): string | null => {
    const token = localStorage.getItem('accessToken');
    if (!token) return null;

    try {
        const decoded: any = jwtDecode(token);
        return decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] 
               || decoded.role 
               || null;
    } catch (error) {
        console.error("Greška pri dekodiranju tokena:", error);
        return null;
    }
};

export const isAdmin = (): boolean => {
    const role = getUserRole();
    return role?.toLowerCase() === 'admin';
};