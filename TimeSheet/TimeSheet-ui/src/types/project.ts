export enum ProjectStatus {
    ACTIVE = 1,
    INACTIVE = 2,
    ARCHIVED = 3
}

export interface ProjectDTO {
    id: string;
    name: string;
    description: string;
    status: ProjectStatus;
    
    clientName: string;
    currentLeadName?: string | null;
    teamMembers: string[];
}

export interface ProjectRequestDTO {
    name: string;
    description: string;
    status: ProjectStatus;
    
    clientId: string;
    currentLeadId?: string | null; 
    teamMemberIds: string[];
}