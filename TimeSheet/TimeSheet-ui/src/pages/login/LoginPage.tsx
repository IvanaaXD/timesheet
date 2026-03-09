import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService';
import './LoginPage.css';
import logoImage from '../../assets/logo.png';

export const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setIsLoading(true);

        try {
            await authService.login({ username, password });
            
            navigate('/');
        } catch (err: any) {
            setError('Pogrešan email ili lozinka. Pokušajte ponovo.');
            console.error('Login error:', err);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-page-container">
            <div className="background-glow"></div>

            <div className="login-content">
                <div className="login-logo">
                    <img src={logoImage} alt="Vega IT Sourcing" />
                </div>

                <div className="login-card">
                    <h2 className="login-title">LOGIN</h2>
                    
                    <form onSubmit={handleLogin}>
                        {error && <div className="error-message">{error}</div>}

                        <div className="form-group">
                            <input
                                type="text"
                                placeholder="Email"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </div>

                        <div className="form-group">
                            <input
                                type="password"
                                placeholder="Password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>

                        <div className="form-footer">
                            <div className="remember-me">
                                <input type="checkbox" id="remember" />
                                <label htmlFor="remember">Remember me</label>
                            </div>
                            
                            <div className="actions">
                                <a href="#" className="forgot-password">Forgot password?</a>
                                <button type="submit" className="login-button" disabled={isLoading}>
                                    {isLoading ? 'Logovanje...' : 'Login'}
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};