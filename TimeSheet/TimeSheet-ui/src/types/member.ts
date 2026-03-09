export enum MemberStatus {
    ACTIVE = 1,
    INACTIVE = 2
}

export enum MemberRole {
    ADMIN = 1,
    WORKER = 2
}

export interface MemberDTO {
    id: string;
    name: string;
    username: string;
    email: string;
    hoursPerWeek: number; 
    status: MemberStatus;
    role: MemberRole;
    projectNames: string[];
}

export interface MemberRequestDTO {
    name: string;
    username: string;
    email: string;
    hoursPerWeek: number;
    status: MemberStatus;
    role: MemberRole;
}