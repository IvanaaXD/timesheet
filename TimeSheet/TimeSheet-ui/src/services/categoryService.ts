import api from '../config/axios';
import { CategoryDTO } from '../types/category';

const BASE_URL = '/api/Category';

export const categoryService = {

    getAllCategories: async (): Promise<CategoryDTO[]> => {
        const response = await api.get<CategoryDTO[]>(BASE_URL);
        return response.data;
    },

    getCategoryById: async (id: string): Promise<CategoryDTO> => {
        const response = await api.get<CategoryDTO>(`${BASE_URL}/id/${id}`);
        return response.data;
    }

    // getCategoryByName: async (name: string): Promise<CategoryDTO> => {
    //     const response = await api.get<CategoryDTO>(`${BASE_URL}/name/${name}`);
    //     return response.data;
    // }
};