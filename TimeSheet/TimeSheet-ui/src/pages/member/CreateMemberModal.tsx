import React, { useState, useEffect } from 'react';
import { MemberRequestDTO, MemberStatus, MemberRole } from '../../types/member';
import { memberService } from '../../services/memberService';
import './CreateMemberModal.css';

interface CreateMemberModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
}

export const CreateMemberModal: React.FC<CreateMemberModalProps> = ({ isOpen, onClose, onSuccess }) => {
    const [formData, setFormData] = useState<MemberRequestDTO>({
        name: '',
        hoursPerWeek: 0,
        username: '',
        email: '',
        status: MemberStatus.ACTIVE,
        role: MemberRole.WORKER
    });

    // Resetujemo formu pri svakom otvaranju
    useEffect(() => {
        if (isOpen) {
            setFormData({
                name: '',
                hoursPerWeek: 0,
                username: '',
                email: '',
                status: MemberStatus.ACTIVE,
                role: MemberRole.WORKER
            });
        }
    }, [isOpen]);

    if (!isOpen) return null;

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value, type } = e.target;
        const val = type === 'number' ? Number(value) : value;
        setFormData(prev => ({ ...prev, [name]: val }));
    };

    const handleRadioChange = (name: string, value: number) => {
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await memberService.createMember(formData);
            onSuccess();
        } catch (error) {
            console.error("Error creating member:", error);
            alert("Failed to create team member.");
        }
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>Create new team member</h2>
                    <button className="close-x-btn" onClick={onClose}>✕</button>
                </div>

                <form onSubmit={handleSubmit} className="modal-form">
                    <div className="input-group">
                        <label>Name:</label>
                        <input name="name" type="text" value={formData.name} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>Hours per week:</label>
                        <input name="hoursPerWeek" type="number" value={formData.hoursPerWeek} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>Username:</label>
                        <input name="username" type="text" value={formData.username} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>Email:</label>
                        <input name="email" type="email" value={formData.email} onChange={handleInputChange} required />
                    </div>

                    <div className="radio-selection-group">
                        <div className="input-group">
                            <label>Status:</label>
                            <div className="radio-options">
                                <label>
                                    <input 
                                        type="radio" 
                                        name="status" 
                                        checked={formData.status === MemberStatus.INACTIVE} 
                                        onChange={() => handleRadioChange('status', MemberStatus.INACTIVE)} 
                                    /> Inactive:
                                </label>
                                <label>
                                    <input 
                                        type="radio" 
                                        name="status" 
                                        checked={formData.status === MemberStatus.ACTIVE} 
                                        onChange={() => handleRadioChange('status', MemberStatus.ACTIVE)} 
                                    /> Active:
                                </label>
                            </div>
                        </div>

                        <div className="input-group">
                            <label>Role:</label>
                            <div className="radio-options">
                                <label>
                                    <input 
                                        type="radio" 
                                        name="role" 
                                        checked={formData.role === MemberRole.ADMIN} 
                                        onChange={() => handleRadioChange('role', MemberRole.ADMIN)} 
                                    /> Admin:
                                </label>
                                <label>
                                    <input 
                                        type="radio" 
                                        name="role" 
                                        checked={formData.role === MemberRole.WORKER} 
                                        onChange={() => handleRadioChange('role', MemberRole.WORKER)} 
                                    /> Worker:
                                </label>
                            </div>
                        </div>
                    </div>

                    <div className="modal-footer">
                        <button type="submit" className="btn-invite">Invite team member</button>
                    </div>
                </form>
            </div>
        </div>
    );
};