import axios from 'axios';

const baseURL = 'http://localhost:5125';

const api = axios.create({
    baseURL: baseURL, 
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken');

        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {

        const originalRequest = error.config;

        if (error.response && error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true; 

            const accessToken = localStorage.getItem('accessToken');
            const refreshToken = localStorage.getItem('refreshToken');

            if (accessToken && refreshToken) {
                try {
                    const response = await axios.post(`${baseURL}/api/Auth/refresh`, {
                        accessToken: accessToken,
                        refreshToken: refreshToken
                    });

                    const newTokens = response.data;

                    localStorage.setItem('accessToken', newTokens.accessToken);
                    localStorage.setItem('refreshToken', newTokens.refreshToken);

                    originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;

                    return api(originalRequest);
                } catch (refreshError) {
                    console.error('Refresh token is invalid or expired. Logging out...');
                    localStorage.removeItem('accessToken');
                    localStorage.removeItem('refreshToken');
                    localStorage.removeItem('user');
                    window.location.href = '/login';
                    return Promise.reject(refreshError);
                }
            } else {
                window.location.href = '/login';
            }
        }
        
        return Promise.reject(error);
    }
);

export default api;