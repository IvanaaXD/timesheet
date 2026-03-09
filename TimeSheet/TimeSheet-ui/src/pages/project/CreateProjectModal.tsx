import React, { useState, useEffect } from 'react';
import { ProjectRequestDTO, ProjectStatus } from '../../types/project';
import { ClientDTO } from '../../types/client';
import { MemberDTO } from '../../types/member';
import { projectService } from '../../services/projectService';
import './CreateProjectModal.css';

interface CreateProjectModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
    clients: ClientDTO[];
    members: MemberDTO[];
}

export const CreateProjectModal: React.FC<CreateProjectModalProps> = ({ 
    isOpen, onClose, onSuccess, clients, members 
}) => {
    const [formData, setFormData] = useState<ProjectRequestDTO>({
        name: '',
        description: '',
        status: ProjectStatus.ACTIVE,
        clientId: '',
        currentLeadId: null,
        teamMemberIds: [] 
    });

    useEffect(() => {
        if (isOpen) {
            setFormData({
                name: '',
                description: '',
                status: ProjectStatus.ACTIVE,
                clientId: '',
                currentLeadId: null,
                teamMemberIds: []
            });
        }
    }, [isOpen]);

    if (!isOpen) return null;

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!formData.clientId) {
            alert("You have to choose a client!");
            return;
        }

        const requestData: ProjectRequestDTO = {
            name: formData.name,
            description: formData.description,
            status: Number(formData.status) as ProjectStatus, 
            clientId: formData.clientId,
            currentLeadId: formData.currentLeadId === "" ? null : formData.currentLeadId,
            teamMemberIds: [] 
        };

        try {
            await projectService.createProject(requestData);
            onSuccess();
        } catch (error: any) {
            alert("Error while creating project.");
        }
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>Create new project</h2>
                    <button className="close-x-btn" onClick={onClose}>✕</button>
                </div>

                <form onSubmit={handleSubmit} className="modal-form">
                    <div className="input-group">
                        <label>Project name:</label>
                        <input 
                            name="name" 
                            type="text" 
                            value={formData.name} 
                            onChange={handleInputChange} 
                            required 
                        />
                    </div>

                    <div className="input-group">
                        <label>Description:</label>
                        <input 
                            name="description" 
                            type="text" 
                            value={formData.description} 
                            onChange={handleInputChange} 
                            required 
                        />
                    </div>

                    <div className="input-group">
                        <label>Customer:</label>
                        <select 
                            name="clientId" 
                            value={formData.clientId} 
                            onChange={handleInputChange} 
                            required
                        >
                            <option value="">Select customer</option>
                            {clients.map(c => (
                                <option key={c.id} value={c.id}>{c.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className="input-group">
                        <label>Lead:</label>
                        <select 
                            name="currentLeadId" 
                            value={formData.currentLeadId || ''} 
                            onChange={handleInputChange}
                        >
                            <option value="">Select lead</option>
                            {members.map(m => (
                                <option key={m.id} value={m.id}>{m.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className="modal-footer">
                        <button type="submit" className="btn-save">Save</button>
                    </div>
                </form>
            </div>
        </div>
    );
};