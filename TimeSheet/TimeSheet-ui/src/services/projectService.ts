import api from '../config/axios';
import { ProjectDTO, ProjectRequestDTO } from '../types/project';
import { PagedListDTO, PagedResult } from '../types/common';

const BASE_URL = '/api/Project';

export const projectService = {

    getProjectById: async (id: string): Promise<ProjectDTO> => {
        const response = await api.get<ProjectDTO>(`${BASE_URL}/id/${id}`);
        return response.data;
    },

    getAllProjects: async (): Promise<ProjectDTO[]> => {
        const response = await api.get<ProjectDTO[]>(BASE_URL);
        return response.data;
    },

    getPagedProjects: async (params: PagedListDTO): Promise<PagedResult<ProjectDTO>> => {
        const response = await api.get<PagedResult<ProjectDTO>>(`${BASE_URL}/paged`, { params });
        return response.data;
    },

    createProject: async (data: ProjectRequestDTO): Promise<ProjectDTO> => {
        const response = await api.post<ProjectDTO>(BASE_URL, data);
        return response.data;
    },

    updateProject: async (id: string, data: ProjectRequestDTO): Promise<ProjectDTO> => {
        const response = await api.put<ProjectDTO>(`${BASE_URL}/${id}`, data);
        return response.data;
    },

    deleteProject: async (id: string): Promise<void> => {
        await api.delete(`${BASE_URL}/${id}`);
    }
};