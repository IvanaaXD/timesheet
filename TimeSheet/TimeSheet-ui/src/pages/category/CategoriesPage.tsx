import React, { useEffect, useState } from 'react';
import { categoryService } from '../../services/categoryService';
import { CategoryDTO } from '../../types/category';
import './CategoriesPage.css';

export const CategoriesPage: React.FC = () => {
    const [categories, setCategories] = useState<CategoryDTO[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const data = await categoryService.getAllCategories();
                setCategories(data);
            } catch (err) {
                console.error("Error while loading categories:", err);
                setError("It's not possible to load categoires at this momment.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchCategories();
    }, []);

    return (
        <div className="page-container">
            <div className="page-header">
                <h1 className="page-title">Categories</h1>
            </div>

            {isLoading ? (
                <div className="loading-spinner">Loading categories...</div>
            ) : error ? (
                <div className="error-message">{error}</div>
            ) : (
                <div className="categories-list">
                    {categories.map((category) => (
                        <div key={category.id} className="category-card">
                            <div className="category-info">
                                <span className="category-name">{category.name}</span>
                            </div>
                        </div>
                    ))}
                    
                    {categories.length === 0 && (
                        <div className="empty-state">There are no categories.</div>
                    )}
                </div>
            )}
        </div>
    );
};