import React, { useEffect, useState } from 'react';
import { projectService } from '../../services/projectService';
import { clientService } from '../../services/clientService';
import { memberService } from '../../services/memberService';
import { ProjectDTO, ProjectRequestDTO, ProjectStatus } from '../../types/project'; 
import { ClientDTO } from '../../types/client';
import { MemberDTO } from '../../types/member';
import './ProjectsPage.css';
import { CreateProjectModal } from './CreateProjectModal';
import { isAdmin as checkAdminStatus } from '../../utils/authUtils';
import { projectMemberService } from '../../services/projectMemberService';
import { ManageMembersModal } from './ManageMembersModal';

export const ProjectsPage: React.FC = () => {

    const isUserAdmin = checkAdminStatus();

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

    const [projectMembers, setProjectMembers] = useState<{[key: string]: MemberDTO[]}>({});
    const [isManageMembersOpen, setIsManageMembersOpen] = useState(false);
    const [selectedProjectForTeam, setSelectedProjectForTeam] = useState<ProjectDTO | null>(null);

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

    const toggleExpand = async (project: ProjectDTO) => {
        if (expandedProjectId === project.id) {
            setExpandedProjectId(null);
            setEditFormData(null);
        } else {
            setExpandedProjectId(project.id);
            
            try {
                const team = await projectMemberService.getMembersByProject(project.id);
                setProjectMembers(prev => ({ ...prev, [project.id]: team }));
            } catch (e) { console.error(e); }

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
        if (!isUserAdmin) return;
        const { name, value } = e.target;

        setEditFormData(prev => {
            if (!prev) return null;

            let finalValue: any = value;
            if (name === 'currentLeadId' && value === "") {
                finalValue = null;
            }

            return { ...prev, [name]: finalValue };
        });
    };

    const handleArchiveToggle = (checked: boolean) => {
        if (!isUserAdmin) return;
        setEditFormData(prev => {
            if (!prev) return null;
            return {
                ...prev,
                status: checked ? ProjectStatus.ARCHIVED : ProjectStatus.ACTIVE
            };
        });
    };

    const handleSave = async (id: string) => {
        if (!editFormData || !isUserAdmin) return;
        try {
            await projectService.updateProject(id, editFormData);
            setExpandedProjectId(null);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            alert("Failed to update project.");
        }
    };

    const handleDelete = async (id: string) => {
        if (!isUserAdmin) return;
        if (!window.confirm("Delete this project?")) return;
        try {
            await projectService.deleteProject(id);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            alert("Failed to delete project.");
        }
    };

    const openManageTeam = (project: ProjectDTO) => {
        setSelectedProjectForTeam(project);
        setIsManageMembersOpen(true);
    };

    const handleTeamUpdateSuccess = async () => {
        if (selectedProjectForTeam) {
            try {
                const team = await projectMemberService.getMembersByProject(selectedProjectForTeam.id);
                setProjectMembers(prev => ({ ...prev, [selectedProjectForTeam.id]: team }));
                setIsManageMembersOpen(false);
            } catch (e) { console.error(e); }
        }
    };

    return (
        <div className="page-container">
            <div className="page-header">
                <h1 className="page-title">Projects</h1>
            </div>

            <div className="top-controls">
                {isUserAdmin && (
                    <button className="create-btn" onClick={() => setIsCreateModalOpen(true)}>
                        + Create new project
                    </button>
                )}
                
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
                isOpen={isUserAdmin && isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={handleCreateSuccess}
                clients={clients}   
                members={members}   
            />

            {selectedProjectForTeam && (
                <ManageMembersModal
                    isOpen={isManageMembersOpen}
                    onClose={() => setIsManageMembersOpen(false)}
                    project={selectedProjectForTeam}
                    allMembers={members}
                    currentTeam={projectMembers[selectedProjectForTeam.id] || []}
                    onSuccess={handleTeamUpdateSuccess}
                />
            )}

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
                                                <input 
                                                    name="name" 
                                                    value={editFormData?.name || ''} 
                                                    onChange={handleInputChange} 
                                                    readOnly={!isUserAdmin} 
                                                />
                                            </div>
                                            <div className="input-group">
                                                <label>Description:</label>
                                                <input 
                                                    name="description" 
                                                    value={editFormData?.description || ''} 
                                                    onChange={handleInputChange} 
                                                    readOnly={!isUserAdmin} 
                                                />
                                            </div>
                                            <div className="input-group">
                                                <label>Customer:</label>
                                                <select 
                                                    name="clientId" 
                                                    value={editFormData?.clientId || ''} 
                                                    onChange={handleInputChange}
                                                    disabled={!isUserAdmin} 
                                                >
                                                    <option value="">Select customer</option>
                                                    {clients.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                                                </select>
                                            </div>
                                            <div className="input-group">
                                                <label>Lead:</label>
                                                <select 
                                                    name="currentLeadId" 
                                                    value={editFormData?.currentLeadId || ''} 
                                                    onChange={handleInputChange}
                                                    disabled={!isUserAdmin} 
                                                >
                                                    <option value="">No lead</option> 
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
                                                            disabled={!isUserAdmin}
                                                        /> Active
                                                    </label>
                                                    <label>
                                                        <input 
                                                            type="radio" 
                                                            name="status" 
                                                            value={ProjectStatus.INACTIVE} 
                                                            checked={editFormData?.status === ProjectStatus.INACTIVE} 
                                                            onChange={handleInputChange} 
                                                            disabled={!isUserAdmin} 
                                                        /> Inactive
                                                    </label>
                                                </div>
                                                <div className="archive-option">
                                                    <label>
                                                        <input 
                                                            type="checkbox" 
                                                            checked={editFormData?.status === ProjectStatus.ARCHIVED} 
                                                            onChange={(e) => handleArchiveToggle(e.target.checked)} 
                                                            disabled={!isUserAdmin} 
                                                        /> Archive
                                                    </label>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="project-team-section">
                                            <div className="section-header">
                                                <h3>Project Team</h3>
                                                <button className="manage-team-btn" onClick={() => openManageTeam(project)}>
                                                    ⚙ Manage Team
                                                </button>
                                            </div>
                                            <div className="team-list">
                                                {projectMembers[project.id]?.length > 0 ? (
                                                    projectMembers[project.id].map(m => (
                                                        <span key={m.id} className={`team-badge ${m.name === project.currentLeadName ? 'lead' : ''}`}>
                                                            {m.name} {m.name === project.currentLeadName && "(Lead)"}
                                                        </span>
                                                    ))
                                                ) : (
                                                    <p className="no-members">No members assigned yet.</p>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    
                                    {isUserAdmin && (
                                        <div className="expanded-footer">
                                            <button className="btn-save" onClick={() => handleSave(project.id)}>Save</button>
                                            <button className="btn-delete" onClick={() => handleDelete(project.id)}>Delete</button>
                                        </div>
                                    )}
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