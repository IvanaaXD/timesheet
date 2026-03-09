export interface PagedListDTO {
    pageNumber: number;
    pageSize: number;
    order?: string;
    searchTerm?: string;
    firstLetter?: string; 
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    currentPage: number;
    totalPages: number; 
    pageSize: number;
}