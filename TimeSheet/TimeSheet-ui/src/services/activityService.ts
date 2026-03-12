import api from '../config/axios';
import { 
    ActivityDTO, 
    ActivityRequestDTO, 
    ActivitySummaryDTO, 
    ReportQueryDTO 
} from '../types/activity';
import { PagedResult } from '../types/common';

const BASE_URL = '/api/Activity';

export const activityService = {

    getActivityById: async (id: string): Promise<ActivityDTO> => {
        const response = await api.get<ActivityDTO>(`${BASE_URL}/id/${id}`);
        return response.data;
    },

    getActivitiesByDate: async (startDate: string, endDate: string): Promise<ActivitySummaryDTO> => {
        const response = await api.get<ActivitySummaryDTO>(`${BASE_URL}/by-date`, {
            params: { startDate, endDate }
        });
        return response.data;
    },

    createActivity: async (data: ActivityRequestDTO): Promise<ActivityDTO> => {
        const response = await api.post<ActivityDTO>(BASE_URL, data);
        return response.data;
    },

    searchActivities: async (query: ReportQueryDTO): Promise<PagedResult<ActivityDTO>> => {
        const response = await api.post<PagedResult<ActivityDTO>>(`${BASE_URL}/search`, query);
        return response.data;
    }
};