import React, { useState } from 'react';
import { ClientRequestDTO } from '../../types/client';
import { CountryDTO } from '../../types/country';
import { clientService } from '../../services/clientService';
import './CreateClientModal.css';

interface CreateClientModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
    countries: CountryDTO[];
}

export const CreateClientModal: React.FC<CreateClientModalProps> = ({ isOpen, onClose, onSuccess, countries }) => {
    const [formData, setFormData] = useState<ClientRequestDTO>({
        name: '',
        address: '',
        city: '',
        zip: '',
        countryId: ''
    });

    if (!isOpen) return null;

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await clientService.createClient(formData);
            onSuccess(); // Refresh list and close modal
        } catch (error) {
            console.error("Error creating client:", error);
            alert("Failed to create client.");
        }
    };

    return (
        /* Overlay captures the click to close the modal */
        <div className="modal-overlay" onClick={onClose}>
            {/* stopPropagation ensures clicking inside the modal doesn't trigger onClose */}
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>Create new client</h2>
                    <button className="close-x-btn" onClick={onClose}>✕</button>
                </div>

                <form onSubmit={handleSubmit} className="modal-form">
                    <div className="input-group">
                        <label>Client name:</label>
                        <input name="name" type="text" value={formData.name} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>Address:</label>
                        <input name="address" type="text" value={formData.address} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>City:</label>
                        <input name="city" type="text" value={formData.city} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>Zip/Postal code:</label>
                        <input name="zip" type="text" value={formData.zip} onChange={handleInputChange} required />
                    </div>

                    <div className="input-group">
                        <label>Country:</label>
                        <select name="countryId" value={formData.countryId} onChange={handleInputChange} required>
                            <option value="">Select country</option>
                            {countries.map(c => (
                                <option key={c.id} value={c.id}>{c.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className="modal-footer">
                        <button type="submit" className="btn-save">Save</button>
                    </div>
                </form>
            </div>
        </div>
    );
};