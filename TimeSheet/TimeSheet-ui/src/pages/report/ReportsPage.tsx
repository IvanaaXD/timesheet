import React, { useEffect, useState, useCallback } from 'react';
import { activityService } from '../../services/activityService';
import { projectService } from '../../services/projectService';
import { categoryService } from '../../services/categoryService';
import { clientService } from '../../services/clientService'; 
import { memberService } from '../../services/memberService'; 
import { ActivityDTO, ReportQueryDTO } from '../../types/activity';
import { ProjectDTO } from '../../types/project';
import { CategoryDTO } from '../../types/category';
import { ClientDTO } from '../../types/client'; 
import { MemberDTO } from '../../types/member'; 
import * as XLSX from 'xlsx';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { robotoBase64 } from './fonts/robotoBase64';
import './ReportsPage.css';
import { isAdmin as checkAdminStatus } from '../../utils/authUtils';

export const ReportsPage: React.FC = () => {

    const isUserAdmin = checkAdminStatus();
    const userJson = localStorage.getItem('user');
    const currentUser = userJson ? JSON.parse(userJson) : null;

    const [reportData, setReportData] = useState<ActivityDTO[]>([]);
    const [projects, setProjects] = useState<ProjectDTO[]>([]);
    const [categories, setCategories] = useState<CategoryDTO[]>([]);
    const [clients, setClients] = useState<ClientDTO[]>([]); 
    const [members, setMembers] = useState<MemberDTO[]>([]); 
    const [isLoading, setIsLoading] = useState(false);

    const [pageNumber, setPageNumber] = useState<number>(1);
    const [pageSize] = useState<number>(10); 
    const [totalPages, setTotalPages] = useState<number>(1);
    const [totalCount, setTotalCount] = useState<number>(0);

    const [filters, setFilters] = useState({
        teamMemberId: isUserAdmin ? '' : (currentUser?.id || ''), 
        clientId: '',
        projectId: '',
        categoryId: '',
        startDate: '',
        endDate: ''
    });

    const [exportRange, setExportRange] = useState<'current' | 'all'>('current');

    useEffect(() => {
        const fetchMetadata = async () => {
            try {
                const [p, c, cl, m] = await Promise.all([
                    projectService.getAllProjects(),
                    categoryService.getAllCategories(),
                    clientService.getAllClients(), 
                    memberService.getAllMembers() 
                ]);
                setProjects(p);
                setCategories(c);
                setClients(cl);
                setMembers(m);
            } catch (err) {
                console.error("Error loading metadata", err);
            }
        };
        fetchMetadata();
    }, []);

    const fetchData = useCallback(async (isNewSearch: boolean = false) => {
        if (!filters.startDate || !filters.endDate) {
            if (isNewSearch) alert("Please select both start and end dates.");
            return;
        }

        setIsLoading(true);
        try {
            const activePage = isNewSearch ? 1 : pageNumber;
            if (isNewSearch) setPageNumber(1);

            const query: ReportQueryDTO = {
                memberId: filters.teamMemberId || null,
                clientId: filters.clientId || null,
                projectId: filters.projectId || null,
                categoryId: filters.categoryId || null,
                startDate: filters.startDate,
                endDate: filters.endDate,
                pageNumber: activePage,
                pageSize: pageSize
            };

            const response = await activityService.searchActivities(query);
            setReportData(response.items || []);
            setTotalCount(response.totalCount || 0);
            setTotalPages(response.totalPages || 1);
        } catch (error: any) {
            alert(error.response?.data?.Message || "An error occurred during search.");
        } finally {
            setIsLoading(false);
        }
    }, [filters, pageNumber, pageSize]);

    useEffect(() => {
        if (filters.startDate && filters.endDate) {
            fetchData();
        }
    }, [pageNumber, fetchData]);

    const handleSearchClick = () => fetchData(true);

    const handleReset = () => {
        setFilters({
            teamMemberId: isUserAdmin ? '' : (currentUser?.id || ''),
            clientId: '',
            projectId: '',
            categoryId: '',
            startDate: '',
            endDate: ''
        });
        setReportData([]);
        setPageNumber(1);
    };

    const currentPageTotalHours = reportData.reduce((acc, curr) => acc + curr.time + curr.overTime, 0);

    const handleExport = async (format: 'excel' | 'pdf' | 'print') => {
        let dataToExport: ActivityDTO[] = [];

        if (exportRange === 'current') {
            dataToExport = reportData;
        } else {
            setIsLoading(true);
            try {
                let allItems: ActivityDTO[] = [];
                
                for (let i = 1; i <= totalPages; i++) {
                    const query: ReportQueryDTO = {
                        memberId: filters.teamMemberId || null,
                        clientId: filters.clientId || null,
                        projectId: filters.projectId || null,
                        categoryId: filters.categoryId || null,
                        startDate: filters.startDate,
                        endDate: filters.endDate,
                        pageNumber: i,      
                        pageSize: pageSize  
                    };

                    const response = await activityService.searchActivities(query);
                    if (response.items) {
                        allItems = [...allItems, ...response.items];
                    }
                }
                
                dataToExport = allItems;

            } catch (error) {
                console.error("Export error:", error);
                alert("Error gathering data from all pages.");
                return;
            } finally {
                setIsLoading(false);
            }
        }

        const totalToDisplay = dataToExport.reduce((acc, curr) => acc + curr.time + curr.overTime, 0);

        if (format === 'excel') exportToExcel(dataToExport, totalToDisplay);
        else if (format === 'pdf') createPDF(dataToExport, totalToDisplay);
        else if (format === 'print') handlePrint(dataToExport, totalToDisplay);
    };
    
    const exportToExcel = (data: ActivityDTO[], total: number) => {
        const worksheetData = data.map(item => ({
            Date: new Date(item.date).toLocaleDateString(),
            'Team Member': item.memberName,
            Project: item.projectName,
            Category: item.categoryName,
            Description: item.description,
            'Time (h)': item.time + item.overTime
        }));
        
        worksheetData.push({ Date: '', 'Team Member': '', Project: '', Category: '', Description: 'TOTAL:', 'Time (h)': total } as any);

        const worksheet = XLSX.utils.json_to_sheet(worksheetData);
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Report");
        XLSX.writeFile(workbook, `TimeSheet_Report.xlsx`);
    };

    const createPDF = (data: ActivityDTO[], total: number) => {
        const doc = new jsPDF();
        doc.addFileToVFS('Roboto-Regular.ttf', robotoBase64);
        doc.addFont('Roboto-Regular.ttf', 'Roboto', 'normal');
        doc.setFont('Roboto'); 
        doc.text("TimeSheet Report", 14, 15);

        const tableRows = data.map(item => [
            new Date(item.date).toLocaleDateString(),
            item.memberName,
            item.projectName,
            item.categoryName,
            item.description,
            (item.time + item.overTime).toString()
        ]);

        autoTable(doc, {
            head: [["Date", "Team member", "Projects", "Categories", "Description", "Time"]],
            body: tableRows,
            startY: 25,
            styles: { font: 'Roboto' },
            headStyles: { fillColor: [243, 108, 33] }
        });

        const finalY = (doc as any).lastAutoTable.finalY;
        doc.text(`Report total: ${total}h`, 14, finalY + 10);
        doc.save(`TimeSheet_Report.pdf`);
    };

    const handlePrint = (data: ActivityDTO[], total: number) => {
        if (exportRange === 'all') {
            alert("For full report printing, please use 'Create PDF' and print the document.");
            return;
        }
        window.print();
    };

    return (
        <div className="reports-container">
            <div className="reports-header">
                <h1 className="page-title">Reports</h1>
            </div>

            <div className="filter-card">
                <div className="filter-grid">
                    <div className="filter-group">
                        <label>Team member:</label>
                        <select 
                            value={filters.teamMemberId} 
                            onChange={(e) => setFilters({...filters, teamMemberId: e.target.value})}
                            disabled={!isUserAdmin} 
                        >
                            <option value="">All</option>
                            {members.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
                        </select>
                    </div>

                    <div className="filter-group">
                        <label>Client:</label>
                        <select value={filters.clientId} onChange={(e) => setFilters({...filters, clientId: e.target.value})}>
                            <option value="">All</option>
                            {clients.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Project:</label>
                        <select value={filters.projectId} onChange={(e) => setFilters({...filters, projectId: e.target.value})}>
                            <option value="">All</option>
                            {projects.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Category:</label>
                        <select value={filters.categoryId} onChange={(e) => setFilters({...filters, categoryId: e.target.value})}>
                            <option value="">All</option>
                            {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Start date:</label>
                        <input type="date" value={filters.startDate} onChange={(e) => setFilters({...filters, startDate: e.target.value})} />
                    </div>
                    <div className="filter-group">
                        <label>End date:</label>
                        <input type="date" value={filters.endDate} onChange={(e) => setFilters({...filters, endDate: e.target.value})} />
                    </div>
                </div>
                <div className="filter-actions">
                    <button className="reset-btn" onClick={handleReset}>Reset</button>
                    <button className="search-btn" onClick={handleSearchClick} disabled={isLoading}>
                        {isLoading ? 'Searching...' : 'Search'}
                    </button>
                </div>
            </div>

            <table className="reports-table">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Team member</th>
                        <th>Projects</th>
                        <th>Categories</th>
                        <th>Description</th>
                        <th className="text-right">Time</th>
                    </tr>
                </thead>
                <tbody>
                    {reportData.map(item => (
                        <tr key={item.id}>
                            <td>{new Date(item.date).toLocaleDateString()}</td>
                            <td>{item.memberName}</td>
                            <td>{item.projectName}</td>
                            <td>{item.categoryName}</td>
                            <td>{item.description}</td>
                            <td className="text-right">{item.time + item.overTime}</td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="pagination">
                <button className="page-btn" disabled={pageNumber === 1 || isLoading} onClick={() => setPageNumber(prev => prev - 1)}>Previous</button>
                <span className="page-info">Page {pageNumber} of {totalPages}</span>
                <button className="page-btn" disabled={pageNumber >= totalPages || isLoading} onClick={() => setPageNumber(prev => prev + 1)}>Next</button>
            </div>

            <div className="reports-footer">
                <div className="export-container">
                    <div className="export-range-selector">
                        <span>Export scope:</span>
                        <label>
                            <input type="radio" name="range" checked={exportRange === 'current'} onChange={() => setExportRange('current')} /> Current Page
                        </label>
                        <label>
                            <input type="radio" name="range" checked={exportRange === 'all'} onChange={() => setExportRange('all')} /> All Results
                        </label>
                    </div>
                    <div className="export-actions">
                        <button className="export-btn" onClick={() => handleExport('print')}>Print report</button>
                        <button className="export-btn" onClick={() => handleExport('pdf')}>Create PDF</button>
                        <button className="export-btn" onClick={() => handleExport('excel')}>Export to excel</button>
                    </div>
                </div>
                <div className="report-total">
                    Report total: <span>{currentPageTotalHours}h</span>
                </div>
            </div>
        </div>
    );
};