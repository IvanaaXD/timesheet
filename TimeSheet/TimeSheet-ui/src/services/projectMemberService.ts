import api from '../config/axios';
import { ProjectMemberRequestDTO } from '../types/projectMember';
import { ProjectDTO } from '../types/project'; 
import { MemberDTO } from '../types/member';   

const BASE_URL = '/api/ProjectMember';

export const projectMemberService = {

    addMemberToProject: async (data: ProjectMemberRequestDTO): Promise<{ message: string }> => {
        const response = await api.post<{ message: string }>(`${BASE_URL}/add`, data);
        return response.data;
    },

    assignLead: async (data: ProjectMemberRequestDTO): Promise<{ message: string }> => {
        const response = await api.post<{ message: string }>(`${BASE_URL}/assign-lead`, data);
        return response.data;
    },

    removeLead: async (data: ProjectMemberRequestDTO): Promise<{ message: string }> => {
        const response = await api.post<{ message: string }>(`${BASE_URL}/remove-lead`, data);
        return response.data;
    },

    removeMemberFromProject: async (projectId: string, memberId: string): Promise<{ message: string }> => {
        const response = await api.delete<{ message: string }>(`${BASE_URL}/project/${projectId}/member/${memberId}`);
        return response.data;
    },

    getProjectsByMember: async (memberId: string): Promise<ProjectDTO[]> => {
        const response = await api.get<ProjectDTO[]>(`${BASE_URL}/member/${memberId}/projects`);
        return response.data;
    },

    getMembersByProject: async (projectId: string): Promise<MemberDTO[]> => {
        const response = await api.get<MemberDTO[]>(`${BASE_URL}/project/${projectId}/members`);
        return response.data;
    }
};