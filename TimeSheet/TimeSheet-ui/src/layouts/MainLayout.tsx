import React from 'react';
import { Outlet, Navigate } from 'react-router-dom';
import { Navbar } from '../components/navbar/Navbar';

export const MainLayout: React.FC = () => {
    const token = localStorage.getItem('accessToken');

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    return (
        <div className="app-container">
            <Navbar />
            <main className="content-area">
                <Outlet />
            </main>
        </div>
    );
};