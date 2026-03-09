export interface ClientDTO {
    id: string;
    name: string;
    address: string;
    city: string;
    zip: string;
    countryName: string;
}

export interface ClientRequestDTO {
    name: string;
    address: string;
    city: string;
    zip: string;
    countryId: string;
}