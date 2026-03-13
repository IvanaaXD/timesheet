import api from '../config/axios';
import { MemberDTO, MemberRequestDTO } from '../types/member';
import { PagedListDTO, PagedResult } from '../types/common';

const BASE_URL = '/api/Member';

export const memberService = {

    getMemberById: async (id: string): Promise<MemberDTO> => {
        const response = await api.get<MemberDTO>(`${BASE_URL}/id/${id}`);
        return response.data;
    },

    getAllMembers: async (): Promise<MemberDTO[]> => {
        const response = await api.get<MemberDTO[]>(BASE_URL);
        return response.data;
    },

    getAllMembersPaged: async (params: PagedListDTO): Promise<PagedResult<MemberDTO>> => {
        const response = await api.get<PagedResult<MemberDTO>>(`${BASE_URL}/paged`, { params });
        return response.data;
    },

    createMember: async (data: MemberRequestDTO): Promise<MemberDTO> => {
        const response = await api.post<MemberDTO>(BASE_URL, data);
        return response.data;
    },

    updateMember: async (id: string, data: MemberRequestDTO): Promise<MemberDTO> => {
        const response = await api.put<MemberDTO>(`${BASE_URL}/${id}`, data);
        return response.data;
    },

    updateMemberPassword: async (id: string): Promise<MemberDTO> => {
        const response = await api.put<MemberDTO>(`${BASE_URL}/password/${id}`);
        return response.data;
    },

    forgotPassword: async (username: string): Promise<MemberDTO> => {
        const response = await api.put<MemberDTO>(`${BASE_URL}/forgot-password/${username}`);
        return response.data;
    },

    deleteMember: async (id: string): Promise<void> => {
        await api.delete(`${BASE_URL}/${id}`);
    }
};