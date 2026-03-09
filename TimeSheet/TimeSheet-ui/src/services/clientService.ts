import api from '../config/axios';
import { ClientDTO, ClientRequestDTO } from '../types/client';
import { PagedListDTO, PagedResult } from '../types/common';

const BASE_URL = '/api/Client';

export const clientService = {

    getClientById: async (id: string): Promise<ClientDTO> => {
        const response = await api.get<ClientDTO>(`${BASE_URL}/id/${id}`);
        return response.data;
    },

    getAllClients: async (): Promise<ClientDTO[]> => {
        const response = await api.get<ClientDTO[]>(BASE_URL);
        return response.data;
    },

    getPagedClients: async (params: PagedListDTO): Promise<PagedResult<ClientDTO>> => {
        const response = await api.get<PagedResult<ClientDTO>>(`${BASE_URL}/paged`, { params });
        return response.data;
    },

    createClient: async (data: ClientRequestDTO): Promise<ClientDTO> => {
        const response = await api.post<ClientDTO>(BASE_URL, data);
        return response.data;
    },

    updateClient: async (id: string, data: ClientRequestDTO): Promise<ClientDTO> => {
        const response = await api.put<ClientDTO>(`${BASE_URL}/${id}`, data);
        return response.data;
    },

    deleteClient: async (id: string): Promise<void> => {
        await api.delete(`${BASE_URL}/${id}`);
    }
};