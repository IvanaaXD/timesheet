import api from '../config/axios'; 
import { CountryDTO } from '../types/country';

const BASE_URL = '/api/Country';

export const countryService = {

    getAllCountries: async (): Promise<CountryDTO[]> => {
        const response = await api.get<CountryDTO[]>(BASE_URL);
        return response.data;
    },

    getCountryByName: async (name: string): Promise<CountryDTO> => {
        const response = await api.get<CountryDTO>(`${BASE_URL}/name/${name}`);
        return response.data;
    }

    // getCountryById: async (id: string): Promise<CountryDTO> => {
    //     const response = await api.get<CountryDTO>(`${BASE_URL}/id/${id}`);
    //     return response.data;
    // }
};