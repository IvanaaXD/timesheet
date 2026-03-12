export interface ActivityDTO {
    id: string;
    description: string;
    date: string;
    time: number;
    overTime: number;
    
    projectName: string;
    clientName: string;
    categoryName: string;
    memberName: string;
}

export interface ActivityRequestDTO {
    description: string;
    date: string;
    time: number;
    overTime: number;
    
    projectId: string;
    categoryId: string;
}

export interface ActivitySummaryDTO {
    activities: ActivityDTO[];
    totalHours: number;
}

export interface ReportQueryDTO {
    memberId?: string | null;
    clientId?: string | null;
    projectId?: string | null;
    categoryId?: string | null;
    startDate?: string | null; 
    endDate?: string | null;  
    pageNumber: number;
    pageSize: number;
}