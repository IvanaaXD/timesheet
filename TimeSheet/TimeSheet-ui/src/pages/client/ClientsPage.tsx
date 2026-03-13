import React, { useEffect, useState } from 'react';
import { clientService } from '../../services/clientService';
import { ClientDTO, ClientRequestDTO } from '../../types/client';
import { countryService } from '../../services/countryService';
import { CountryDTO } from '../../types/country';
import './ClientsPage.css';
import { CreateClientModal } from './CreateClientModal';
import { isAdmin as checkAdminStatus } from '../../utils/authUtils';

export const ClientsPage: React.FC = () => {

    const isAdmin = checkAdminStatus();
    
    const [clients, setClients] = useState<ClientDTO[]>([]);
    const [countries, setCountries] = useState<CountryDTO[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    
    const [expandedClientId, setExpandedClientId] = useState<string | null>(null);
    const [editFormData, setEditFormData] = useState<ClientRequestDTO | null>(null);
    
    const [refreshTrigger, setRefreshTrigger] = useState<number>(0);

    const [pageNumber, setPageNumber] = useState<number>(1);
    const [pageSize, setPageSize] = useState<number>(10); 
    const [totalPages, setTotalPages] = useState<number>(1);
    const [totalCount, setTotalCount] = useState<number>(0);

    const [searchTerm, setSearchTerm] = useState<string>('');
    const [selectedLetter, setSelectedLetter] = useState<string | null>(null);

    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

    const handleCreateSuccess = () => {
        setIsCreateModalOpen(false);
        setRefreshTrigger(prev => prev + 1);
    };

    const alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".split('');

    useEffect(() => {
        const fetchCountries = async () => {
            try {
                const data = await countryService.getAllCountries();
                setCountries(data);
            } catch (error) {
                console.error("Error loading countries:", error);
            }
        };
        fetchCountries();
    }, []);

    useEffect(() => {
        setPageNumber(1);
        setExpandedClientId(null); 
    }, [searchTerm, selectedLetter]);

    useEffect(() => {
        const fetchClients = async () => {
            setIsLoading(true);
            try {
                const response = await clientService.getPagedClients({
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    searchTerm: searchTerm, 
                    firstLetter: selectedLetter || undefined 
                });
                
                setClients(response.items || []); 
                setTotalCount(response.totalCount || 0);
                setTotalPages(response.totalPages || 1);
            } catch (error) {
                console.error("Error loading clients:", error);
            } finally {
                setIsLoading(false);
            }
        };
        
        fetchClients();
    }, [pageNumber, pageSize, searchTerm, selectedLetter, refreshTrigger]);

    const toggleExpand = (client: ClientDTO) => {
        if (expandedClientId === client.id) {
            setExpandedClientId(null);
            setEditFormData(null);
        } else {
            setExpandedClientId(client.id);
            
            const countryId = countries.find(c => c.name === client.countryName)?.id || '';
            
            setEditFormData({
                name: client.name,
                address: client.address || '',
                city: client.city || '',
                zip: client.zip || '',
                countryId: countryId
            });
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        if (!isAdmin) return;

        const { name, value } = e.target;
        setEditFormData(prev => prev ? { ...prev, [name]: value } : null);
    };

    const handleSave = async (id: string) => {
        if (!editFormData || !isAdmin) return;
        
        try {
            await clientService.updateClient(id, editFormData);
            setExpandedClientId(null); 
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error updating client:", error);
            alert("Failed to update client. Please try again.");
        }
    };

    const handleDelete = async (id: string, event?: React.MouseEvent) => {
        if (!isAdmin) return;
        if (event) event.stopPropagation(); 
        
        const confirmed = window.confirm("Are you sure you want to delete this client?");
        if (!confirmed) return;

        try {
            await clientService.deleteClient(id);
            setExpandedClientId(null);
            setRefreshTrigger(prev => prev + 1); 
        } catch (error) {
            console.error("Error deleting client:", error);
            alert("Failed to delete client. Please try again.");
        }
    };

    return (
        <div className="page-container">
            <div className="page-header">
                <h1 className="page-title">Clients</h1>
            </div>

            <div className="top-controls">
                {isAdmin && (
                    <button className="create-btn" onClick={() => setIsCreateModalOpen(true)}>
                        + Create new client
                    </button>
                )}
                
                <div className="search-box">
                    <input 
                        type="text" 
                        placeholder="Search clients..." 
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>
            </div>

            <CreateClientModal 
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={handleCreateSuccess}
                countries={countries}
            />

            <div className="alphabet-filter">
                <button 
                    className={`letter-btn ${selectedLetter === null ? 'active' : ''}`}
                    onClick={() => setSelectedLetter(null)}
                >
                    All
                </button>
                {alphabet.map(letter => (
                    <button 
                        key={letter}
                        className={`letter-btn ${selectedLetter === letter ? 'active' : ''}`}
                        onClick={() => setSelectedLetter(selectedLetter === letter ? null : letter)}
                    >
                        {letter}
                    </button>
                ))}
            </div>

            {isLoading ? (
                <div className="loading-spinner">Loading clients...</div>
            ) : (
                <div className="clients-list">
                    {clients.map((client) => (
                        <React.Fragment key={client.id}>
                            {expandedClientId === client.id ? (
                                <div className="client-card expanded">
                                    <div 
                                        className="expanded-header" 
                                        onClick={() => setExpandedClientId(null)}
                                    >
                                        <span className="client-name">{client.name}</span>
                                        <button className="close-expand-btn">✕</button>
                                    </div>
                                    
                                    <div className="expanded-body">
                                        <div className="form-grid">
                                            <div className="input-group">
                                                <label>Client name: </label>
                                                <input 
                                                    type="text" 
                                                    name="name"
                                                    value={editFormData?.name || ''} 
                                                    onChange={handleInputChange}
                                                    readOnly={!isAdmin} 
                                                />
                                            </div>
                                            
                                            <div className="input-group">
                                                <label>Address: </label>
                                                <input 
                                                    type="text" 
                                                    name="address"
                                                    value={editFormData?.address || ''} 
                                                    onChange={handleInputChange}
                                                    readOnly={!isAdmin}
                                                />
                                            </div>
                                            
                                            <div className="input-group">
                                                <label>City: </label>
                                                <input 
                                                    type="text" 
                                                    name="city"
                                                    value={editFormData?.city || ''} 
                                                    onChange={handleInputChange}
                                                    readOnly={!isAdmin}
                                                />
                                            </div>
                                            
                                            <div className="input-group">
                                                <label>Postal code: </label>
                                                <input 
                                                    type="text" 
                                                    name="zip"
                                                    value={editFormData?.zip || ''} 
                                                    onChange={handleInputChange}
                                                    readOnly={!isAdmin}
                                                />
                                            </div>
                                            
                                            <div className="input-group">
                                                <label>Country: </label>
                                                <select 
                                                    name="countryId"
                                                    value={editFormData?.countryId || ''}
                                                    onChange={handleInputChange}
                                                    disabled={!isAdmin} 
                                                >
                                                    <option value="" disabled>Select country</option>
                                                    {countries.map((country) => (
                                                        <option key={country.id} value={country.id}>
                                                            {country.name}
                                                        </option>
                                                    ))}
                                                </select>
                                            </div>
                                        </div>
                                    </div>

                                    {isAdmin && (
                                        <div className="expanded-footer">
                                            <button className="btn-save" onClick={() => handleSave(client.id)}>Save</button>
                                            <button className="btn-delete" onClick={() => handleDelete(client.id)}>Delete</button>
                                        </div>
                                    )}
                                </div>
                            ) : (
                                <div className="client-card" onClick={() => toggleExpand(client)}>
                                    <span className="client-name">{client.name}</span>
                                </div>
                            )}
                        </React.Fragment>
                    ))}

                    {clients.length === 0 && (
                        <div className="empty-state">No clients found.</div>
                    )}
                </div>
            )}

            <div className="pagination">
                <button 
                    className="page-btn" 
                    disabled={pageNumber === 1}
                    onClick={() => setPageNumber(prev => prev - 1)}
                >
                    Previous
                </button>
                
                <span className="page-info">
                    Page {totalPages === 0 ? 0 : pageNumber} of {totalPages}
                </span>
                
                <button 
                    className="page-btn" 
                    disabled={pageNumber >= totalPages || totalPages === 0}
                    onClick={() => setPageNumber(prev => prev + 1)}
                >
                    Next
                </button>
            </div>
        </div>
    );
};

