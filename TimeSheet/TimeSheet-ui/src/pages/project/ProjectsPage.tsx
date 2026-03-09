import React, { useEffect, useState } from 'react';
import { projectService } from '../../services/projectService';
import { clientService } from '../../services/clientService';
import { memberService } from '../../services/memberService';
import { ProjectDTO, ProjectRequestDTO, ProjectStatus } from '../../types/project'; 
import { ClientDTO } from '../../types/client';
import { MemberDTO } from '../../types/member';
import './ProjectsPage.css';
import { CreateProjectModal } from './CreateProjectModal';

export const ProjectsPage: React.FC = () => {

    const [projects, setProjects] = useState<ProjectDTO[]>([]);
    const [clients, setClients] = useState<ClientDTO[]>([]);
    const [members, setMembers] = useState<MemberDTO[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    
    const [expandedProjectId, setExpandedProjectId] = useState<string | null>(null);
    const [editFormData, setEditFormData] = useState<ProjectRequestDTO | null>(null);
    const [refreshTrigger, setRefreshTrigger] = useState<number>(0);

    const [pageNumber, setPageNumber] = useState<number>(1);
    const [pageSize, setPageSize] = useState<number>(10); 
    const [totalPages, setTotalPages] = useState<number>(1);
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [selectedLetter, setSelectedLetter] = useState<string | null>(null);

    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

    const handleCreateSuccess = () => {
        setIsCreateModalOpen(false);
        setRefreshTrigger(prev => prev + 1);
    };

    const alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".split('');

    useEffect(() => {
        const fetchDependencies = async () => {
            try {
                const [clientsData, membersData] = await Promise.all([
                    clientService.getAllClients(),
                    memberService.getAllMembers()
                ]);
                setClients(clientsData);
                setMembers(membersData);
            } catch (error) {
                console.error("Error loading dependencies:", error);
            }
        };
        fetchDependencies();
    }, []);

    useEffect(() => {
        const fetchProjects = async () => {
            setIsLoading(true);
            try {
                const response = await projectService.getPagedProjects({
                    pageNumber,
                    pageSize,
                    searchTerm,
                    firstLetter: selectedLetter || undefined
                });
                setProjects(response.items || []);
                setTotalPages(response.totalPages || 1);
            } catch (error) {
                console.error("Error loading projects:", error);
            } finally {
                setIsLoading(false);
            }
        };
        fetchProjects();
    }, [pageNumber, pageSize, searchTerm, selectedLetter, refreshTrigger]);

    const toggleExpand = (project: ProjectDTO) => {
        if (expandedProjectId === project.id) {
            setExpandedProjectId(null);
            setEditFormData(null);
        } else {
            setExpandedProjectId(project.id);
            
            let initialStatus = project.status;
            if ((project.status as any) === 1) initialStatus = ProjectStatus.ACTIVE;
            if ((project.status as any) === 2) initialStatus = ProjectStatus.INACTIVE;
            if ((project.status as any) === 3) initialStatus = ProjectStatus.ARCHIVED;

            setEditFormData({
                name: project.name,
                description: project.description || '',
                status: initialStatus,
                clientId: clients.find(c => c.name === project.clientName)?.id || '',
                currentLeadId: members.find(m => m.name === project.currentLeadName)?.id || null,
                teamMemberIds: [] 
            });
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        
        setEditFormData(prev => prev ? { ...prev, [name]: value } : null);
    };

    const handleArchiveToggle = (checked: boolean) => {
        setEditFormData(prev => {
            if (!prev) return null;
            return {
                ...prev,
                status: checked ? ProjectStatus.ARCHIVED : ProjectStatus.ACTIVE
            };
        });
    };

    const handleSave = async (id: string) => {
        if (!editFormData) return;
        try {
            await projectService.updateProject(id, editFormData);
            setExpandedProjectId(null);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            alert("Failed to update project.");
        }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Delete this project?")) return;
        try {
            await projectService.deleteProject(id);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            alert("Failed to delete project.");
        }
    };

    return (
        <div className="page-container">
            <div className="page-header">
                <h1 className="page-title">Projects</h1>
            </div>

            <div className="top-controls">
                <button className="create-btn" onClick={() => setIsCreateModalOpen(true)}>
                    + Create new project
                </button>                
                <div className="search-box">
                    <input 
                        type="text" 
                        placeholder="Search projects..." 
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>
            </div>

            <CreateProjectModal 
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={handleCreateSuccess}
                clients={clients}   
                members={members}   
            />

            <div className="alphabet-filter">
                <button className={`letter-btn ${!selectedLetter ? 'active' : ''}`} onClick={() => setSelectedLetter(null)}>All</button>
                {alphabet.map(l => (
                    <button key={l} className={`letter-btn ${selectedLetter === l ? 'active' : ''}`} onClick={() => setSelectedLetter(l)}>{l}</button>
                ))}
            </div>

            {isLoading ? <div className="loading-spinner">Loading projects...</div> : (
                <div className="projects-list">
                    {projects.map((project) => (
                        <React.Fragment key={project.id}>
                            {expandedProjectId === project.id ? (
                                <div className="project-card expanded">
                                    <div 
                                        className="expanded-header" 
                                        onClick={() => setExpandedProjectId(null)}
                                        style={{ cursor: 'pointer' }}
                                    >
                                        <span className="project-name">{project.name} <small>({project.clientName})</small></span>
                                        <button className="close-expand-btn">✕</button>
                                    </div>
                                    <div className="expanded-body">
                                        <div className="form-grid">
                                            <div className="input-group">
                                                <label>Project name:</label>
                                                <input name="name" value={editFormData?.name || ''} onChange={handleInputChange} />
                                            </div>
                                            <div className="input-group">
                                                <label>Description:</label>
                                                <input name="description" value={editFormData?.description || ''} onChange={handleInputChange} />
                                            </div>
                                            <div className="input-group">
                                                <label>Customer:</label>
                                                <select name="clientId" value={editFormData?.clientId || ''} onChange={handleInputChange}>
                                                    <option value="">Select customer</option>
                                                    {clients.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                                                </select>
                                            </div>
                                            <div className="input-group">
                                                <label>Lead:</label>
                                                <select name="currentLeadId" value={editFormData?.currentLeadId || ''} onChange={handleInputChange}>
                                                    <option value="">Select lead</option>
                                                    {members.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
                                                </select>
                                            </div>
                                            <div className="input-group status-group">
                                                <label>Status:</label>
                                                <div className="radio-options">
                                                    <label>
                                                        <input 
                                                            type="radio" 
                                                            name="status" 
                                                            value={ProjectStatus.ACTIVE} 
                                                            checked={editFormData?.status === ProjectStatus.ACTIVE} 
                                                            onChange={handleInputChange} 
                                                        /> Active
                                                    </label>
                                                    <label>
                                                        <input 
                                                            type="radio" 
                                                            name="status" 
                                                            value={ProjectStatus.INACTIVE} 
                                                            checked={editFormData?.status === ProjectStatus.INACTIVE} 
                                                            onChange={handleInputChange} 
                                                        /> Inactive
                                                    </label>
                                                </div>
                                                <div className="archive-option">
                                                    <label>
                                                        <input 
                                                            type="checkbox" 
                                                            checked={editFormData?.status === ProjectStatus.ARCHIVED} 
                                                            onChange={(e) => handleArchiveToggle(e.target.checked)} 
                                                        /> Archive
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="expanded-footer">
                                        <button className="btn-save" onClick={() => handleSave(project.id)}>Save</button>
                                        <button className="btn-delete" onClick={() => handleDelete(project.id)}>Delete</button>
                                    </div>
                                </div>
                            ) : (
                                <div className="project-card" onClick={() => toggleExpand(project)}>
                                    <span className="project-name">{project.name} <small className="cust-small">({project.clientName})</small></span>
                                </div>
                            )}
                        </React.Fragment>
                    ))}
                </div>
            )}
            
            <div className="pagination">
                <button 
                    className="page-btn" 
                    disabled={pageNumber === 1}
                    onClick={() => setPageNumber(prev => prev - 1)}
                >
                    Previous
                </button>
                <span className="page-info" style={{ padding: '8px 15px', color: '#555' }}>
                    Page {totalPages === 0 ? 0 : pageNumber} of {totalPages}
                </span>
                <button 
                    className="page-btn" 
                    disabled={pageNumber >= totalPages || totalPages === 0}
                    onClick={() => setPageNumber(prev => prev + 1)}
                >
                    Next
                </button>
            </div>
        </div>
    );
};