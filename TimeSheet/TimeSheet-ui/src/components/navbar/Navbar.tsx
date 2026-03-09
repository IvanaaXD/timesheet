import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService';
import './Navbar.css'; 
import logoImage from '../../assets/logo.png';

export const Navbar: React.FC = () => {
    const navigate = useNavigate();
    
    const userString = localStorage.getItem('user');
    const user = userString ? JSON.parse(userString) : null;

    const handleLogout = async () => {
        await authService.logout();
        navigate('/login');
    };

    return (
        <header className="navbar-header">
            <div className="top-bar">

                <div className="logo">
                    <img src={logoImage} alt="Vega IT Sourcing" />
                </div>
                
                <div className="user-info">
                    <span className="user-name">{user?.username || 'Guest'}</span>
                    <span className="separator">|</span>
                    <button onClick={handleLogout} className="logout-btn">Logout</button>
                </div>
            </div>

            <nav className="main-nav">
                <NavLink to="/" className={({ isActive }) => isActive ? "nav-item active" : "nav-item"}>
                    TimeSheet
                </NavLink>
                <NavLink to="/clients" className={({ isActive }) => isActive ? "nav-item active" : "nav-item"}>
                    Clients
                </NavLink>
                <NavLink to="/projects" className={({ isActive }) => isActive ? "nav-item active" : "nav-item"}>
                    Projects
                </NavLink>
                <NavLink to="/categories" className={({ isActive }) => isActive ? "nav-item active" : "nav-item"}>
                    Categories
                </NavLink>
                <NavLink to="/team-members" className={({ isActive }) => isActive ? "nav-item active" : "nav-item"}>
                    Team members
                </NavLink>
                <NavLink to="/reports" className={({ isActive }) => isActive ? "nav-item active" : "nav-item"}>
                    Reports
                </NavLink>
            </nav>
        </header>
    );
};