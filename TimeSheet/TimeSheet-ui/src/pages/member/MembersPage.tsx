import React, { useEffect, useState } from 'react';
import { memberService } from '../../services/memberService';
import { MemberDTO, MemberRequestDTO, MemberRole, MemberStatus } from '../../types/member';
import './MembersPage.css';
import { CreateMemberModal } from './CreateMemberModal.tsx';
import { isAdmin as checkAdminStatus } from '../../utils/authUtils';

export const MembersPage: React.FC = () => {

    const isUserAdmin = checkAdminStatus();
    const userJson = localStorage.getItem('user');
    const currentUser = userJson ? JSON.parse(userJson) : null;

    const [members, setMembers] = useState<MemberDTO[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [expandedMemberId, setExpandedMemberId] = useState<string | null>(null);
    const [editFormData, setEditFormData] = useState<MemberRequestDTO | null>(null);
    const [refreshTrigger, setRefreshTrigger] = useState<number>(0);

    const [pageNumber, setPageNumber] = useState<number>(1);
    const [pageSize] = useState<number>(10); 
    const [totalPages, setTotalPages] = useState<number>(1);
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

    useEffect(() => {
        const fetchMembers = async () => {
            setIsLoading(true);
            try {
                const response = await memberService.getAllMembersPaged({
                    pageNumber: pageNumber,
                    pageSize: pageSize
                });
                setMembers(response.items || []);
                setTotalPages(response.totalPages || 1);
            } catch (error) {
                console.error("Error loading members:", error);
            } finally {
                setIsLoading(false);
            }
        };
        fetchMembers();
    }, [pageNumber, refreshTrigger]);

    const toggleExpand = (member: MemberDTO) => {
        if (expandedMemberId === member.id) {
            setExpandedMemberId(null);
            setEditFormData(null);
        } else {
            setExpandedMemberId(member.id);
            setEditFormData({
                name: member.name,
                username: member.username,
                email: member.email,
                hoursPerWeek: member.hoursPerWeek,
                status: member.status,
                role: member.role
            });
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!isUserAdmin) return; 
        const { name, value, type } = e.target;
        const val = type === 'number' ? Number(value) : value;
        setEditFormData(prev => prev ? { ...prev, [name]: val } : null);
    };

    const handleSave = async (id: string) => {
        if (!editFormData || !isUserAdmin) return;
        try {
            await memberService.updateMember(id, editFormData);
            setExpandedMemberId(null);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            alert("Failed to update member.");
        }
    };

    const handleCreateSuccess = () => {
        setIsCreateModalOpen(false);
        setRefreshTrigger(prev => prev + 1);
    };

    const handleDelete = async (id: string) => {
        if (!isUserAdmin) return;
        if (!window.confirm("Delete this member?")) return;
        try {
            await memberService.deleteMember(id);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            alert("Failed to delete member.");
        }
    };

    const handleResetPassword = async (id: string) => {

        const isSelf = currentUser?.id === id;
        if (!isUserAdmin && !isSelf) return;

        if (!window.confirm("Are you sure you want to change the password?")) return;
        
        try {
            await memberService.updateMemberPassword(id);
            
            if (isSelf) {
                alert("You have changed your own password. For security reasons, you will be logged out.");
                localStorage.clear(); 
                window.location.href = '/login'; 
            } else {
                alert("Password changed successfully.");
                setRefreshTrigger(prev => prev + 1);
            }
        } catch (error) {
            console.error("Error resetting password:", error);
            alert("Failed to change password.");
        }
    };

    return (
        <div className="page-container">
            <div className="page-header">
                <h1 className="page-title">Team members</h1>
            </div>

            <div className="top-controls">
                {isUserAdmin && (
                    <button className="create-btn" onClick={() => setIsCreateModalOpen(true)}>
                        + Create new member
                    </button>
                )}
            </div>

            <CreateMemberModal 
                isOpen={isUserAdmin && isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={handleCreateSuccess}
            />

            {isLoading ? <div className="loading-spinner">Loading members...</div> : (
                <div className="members-list">
                    {members.map((member) => {
                        const isSelf = currentUser?.id === member.id;
                        
                        return (
                            <React.Fragment key={member.id}>
                                {expandedMemberId === member.id ? (
                                    <div className="member-card expanded">
                                        <div className="expanded-header" onClick={() => setExpandedMemberId(null)}>
                                            <span className="member-name">{member.name} {isSelf && "(You)"}</span>
                                            <button className="close-expand-btn">✕</button>
                                        </div>
                                        <div className="expanded-body">
                                            <div className="form-grid">
                                                <div className="input-group">
                                                    <label>Name: </label>
                                                    <input name="name" value={editFormData?.name || ''} onChange={handleInputChange} readOnly={!isUserAdmin} />
                                                </div>
                                                <div className="input-group">
                                                    <label>Username: </label>
                                                    <input name="username" value={editFormData?.username || ''} onChange={handleInputChange} readOnly={!isUserAdmin} />
                                                </div>
                                                <div className="input-group">
                                                    <label>Status: </label>
                                                    <div className="radio-group">
                                                        <label><input type="radio" name="status" value={MemberStatus.INACTIVE} checked={editFormData?.status === MemberStatus.INACTIVE} disabled={!isUserAdmin} onChange={() => setEditFormData(p => p ? {...p, status: MemberStatus.INACTIVE} : null)} /> Inactive:</label>
                                                        <label><input type="radio" name="status" value={MemberStatus.ACTIVE} checked={editFormData?.status === MemberStatus.ACTIVE} disabled={!isUserAdmin} onChange={() => setEditFormData(p => p ? {...p, status: MemberStatus.ACTIVE} : null)} /> Active:</label>
                                                    </div>
                                                </div>
                                                <div className="input-group">
                                                    <label>Hours per week: </label>
                                                    <input type="number" name="hoursPerWeek" value={editFormData?.hoursPerWeek || 0} onChange={handleInputChange} readOnly={!isUserAdmin} />
                                                </div>
                                                <div className="input-group">
                                                    <label>Email: </label>
                                                    <input name="email" value={editFormData?.email || ''} onChange={handleInputChange} readOnly={!isUserAdmin} />
                                                </div>
                                                <div className="input-group">
                                                    <label>Role: </label>
                                                    <div className="radio-group">
                                                        <label><input type="radio" name="role" value={MemberRole.ADMIN} checked={editFormData?.role === MemberRole.ADMIN} disabled={!isUserAdmin} onChange={() => setEditFormData(p => p ? {...p, role: MemberRole.ADMIN} : null)} /> Admin:</label>
                                                        <label><input type="radio" name="role" value={MemberRole.WORKER} checked={editFormData?.role === MemberRole.WORKER} disabled={!isUserAdmin} onChange={() => setEditFormData(p => p ? {...p, role: MemberRole.WORKER} : null)} /> Worker:</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="expanded-footer">
                                            {isUserAdmin && (
                                                <>
                                                    <button className="btn-save" onClick={() => handleSave(member.id)}>Save</button>
                                                    <button className="btn-delete" onClick={() => handleDelete(member.id)}>Delete</button>
                                                </>
                                            )}
                                            
                                            {(isUserAdmin || isSelf) && (
                                                <button className="btn-reset" onClick={() => handleResetPassword(member.id)}>Reset Password</button>
                                            )}
                                        </div>
                                    </div>
                                ) : (
                                    <div className="member-card" onClick={() => toggleExpand(member)}>
                                        <span className="member-name">{member.name} {isSelf && <small style={{color: '#999', marginLeft: '10px'}}>(You)</small>}</span>
                                    </div>
                                )}
                            </React.Fragment>
                        );
                    })}
                </div>
            )}
            
            <div className="pagination">
                <button className="page-btn" disabled={pageNumber === 1} onClick={() => setPageNumber(p => p - 1)}>Previous</button>
                <span className="page-info">Page {pageNumber} of {totalPages}</span>
                <button className="page-btn" disabled={pageNumber >= totalPages} onClick={() => setPageNumber(p => p + 1)}>Next</button>
            </div>

        </div>
    );
};