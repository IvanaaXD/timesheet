import React, { useState, useEffect } from 'react';
import { MemberDTO } from '../../types/member';
import { ProjectDTO } from '../../types/project';
import { projectMemberService } from '../../services/projectMemberService';
import { ProjectMemberRequestDTO } from '../../types/projectMember';

interface Props {
    isOpen: boolean;
    onClose: () => void;
    project: ProjectDTO;
    allMembers: MemberDTO[];
    currentTeam: MemberDTO[];
    onSuccess: () => void;
}

export const ManageMembersModal: React.FC<Props> = ({ isOpen, onClose, project, allMembers, currentTeam, onSuccess }) => {

    const [selectedIds, setSelectedIds] = useState<string[]>([]);
    const [leadId, setLeadId] = useState<string | null>(null);
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        if (isOpen) {
            setSelectedIds(currentTeam.map(m => m.id));
            const lead = currentTeam.find(m => m.name === project.currentLeadName);
            setLeadId(lead?.id || null);
        }
    }, [isOpen, currentTeam, project]);

    const handleToggleMember = (id: string) => {
        if (selectedIds.includes(id)) {
            setSelectedIds(prev => prev.filter(x => x !== id));
            if (leadId === id) setLeadId(null);
        } else {
            setSelectedIds(prev => [...prev, id]);
        }
    };

    const handleSave = async () => {
        setIsSaving(true);
        try {
            const removedMembers = currentTeam.filter(m => !selectedIds.includes(m.id));
            for (const m of removedMembers) {
                await projectMemberService.removeMemberFromProject(project.id, m.id);
            }

            const newMembers = selectedIds.filter(id => !currentTeam.find(m => m.id === id));
            for (const id of newMembers) {
                const dto: ProjectMemberRequestDTO = {
                    projectId: project.id,
                    memberId: id,
                    isLead: id === leadId
                };
                await projectMemberService.addMemberToProject(dto);
            }

            const currentLead = currentTeam.find(m => m.name === project.currentLeadName);

            if (!leadId && currentLead) {
                await projectMemberService.removeLead({
                    projectId: project.id,
                    memberId: currentLead.id,
                    isLead: false
                });
            } 
            else if (leadId && leadId !== currentLead?.id) {
                await projectMemberService.assignLead({
                    projectId: project.id,
                    memberId: leadId,
                    isLead: true
                });
            }

            onSuccess();
            onClose();
        } catch (error) {
            console.error("Error saving team:", error);
            alert("Error happend while updating the team.");
        } finally {
            setIsSaving(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content team-manage-modal">
                <div className="modal-header">
                    <h2>Manage Team: {project.name}</h2>
                    <button className="close-x-btn" onClick={onClose}>✕</button>
                </div>
                
                <div className="modal-body">
                    <div className="team-grid-header">
                        <span className="col-name">Member Name</span>
                        <span className="col-check">Member</span>
                        <span className="col-lead">Lead</span>
                    </div>
                    
                    <div className="team-list-scrollable">
                        <div className="team-member-row no-lead-option">
                            <span className="member-name"><em>None (No lead assigned)</em></span>
                            <div className="col-check"></div> 
                            <div className="col-lead">
                                <input 
                                    type="radio" 
                                    name="project-lead"
                                    checked={leadId === null}
                                    onChange={() => setLeadId(null)}
                                />
                            </div>
                        </div>
                        {allMembers.map(m => (
                            <div key={m.id} className="team-member-row">
                                <span className="member-name">{m.name}</span>
                                
                                <div className="col-check">
                                    <input 
                                        type="checkbox" 
                                        checked={selectedIds.includes(m.id)}
                                        onChange={() => handleToggleMember(m.id)}
                                    />
                                </div>
                                
                                <div className="col-lead">
                                    <input 
                                        type="radio" 
                                        name="project-lead"
                                        disabled={!selectedIds.includes(m.id)}
                                        checked={leadId === m.id}
                                        onChange={() => setLeadId(m.id)}
                                    />
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="modal-footer">
                    <button 
                        className="btn-save" 
                        onClick={handleSave} 
                        disabled={isSaving}
                    >
                        {isSaving ? "Saving..." : "Save Changes"}
                    </button>
                </div>
            </div>
        </div>
    );
};