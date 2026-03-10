import React, { useState, useEffect } from 'react';
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


export const ReportsPage: React.FC = () => {
    const [reportData, setReportData] = useState<ActivityDTO[]>([]);
    const [projects, setProjects] = useState<ProjectDTO[]>([]);
    const [categories, setCategories] = useState<CategoryDTO[]>([]);
    const [clients, setClients] = useState<ClientDTO[]>([]); 
    const [members, setMembers] = useState<MemberDTO[]>([]); 
    const [isLoading, setIsLoading] = useState(false);

    const [filters, setFilters] = useState({
        teamMemberId: '', 
        clientId: '',
        projectId: '',
        categoryId: '',
        startDate: '',
        endDate: ''
    });

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
                console.error("Greška pri učitavanju metapodataka", err);
            }
        };
        fetchMetadata();
    }, []);

    const handleSearch = async () => {
        if (!filters.startDate || !filters.endDate) {
            alert("Please select both start and end dates.");
            return;
        }

        setIsLoading(true);
        try {
            const query: ReportQueryDTO = {
                memberId: filters.teamMemberId || null,
                clientId: filters.clientId || null,
                projectId: filters.projectId || null,
                categoryId: filters.categoryId || null,
                startDate: filters.startDate,
                endDate: filters.endDate
            };

            const data = await activityService.searchActivities(query);
            setReportData(data);
        } catch (error: any) {
            const errorMsg = error.response?.data?.Message || "An error occurred during search.";
            alert(errorMsg);
        } finally {
            setIsLoading(false);
        }
    };

    const handleReset = () => {
        setFilters({
            teamMemberId: '',
            clientId: '',
            projectId: '',
            categoryId: '',
            startDate: '',
            endDate: ''
        });
        setReportData([]);
    };

    const totalHours = reportData.reduce((acc, curr) => acc + curr.time + curr.overTime, 0);

    const exportToExcel = () => {
        if (reportData.length === 0) return alert("No data to export!");

        const worksheetData = reportData.map(item => ({
            Date: new Date(item.date).toLocaleDateString(),
            'Team Member': item.memberName,
            Project: item.projectName,
            Category: item.categoryName,
            Description: item.description,
            'Time (h)': item.time + item.overTime
        }));

        worksheetData.push({
            Date: 'TOTAL',
            'Team Member': '',
            Project: '',
            Category: '',
            Description: '',
            'Time (h)': totalHours
        });

        const worksheet = XLSX.utils.json_to_sheet(worksheetData);
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "TimeSheet Report");
        
        XLSX.writeFile(workbook, `TimeSheet_Report_${new Date().toISOString().split('T')[0]}.xlsx`);
    };

    const createPDF = () => {
        if (reportData.length === 0) return alert("No data to export!");

        const doc = new jsPDF();

        doc.addFileToVFS('Roboto-Regular.ttf', robotoBase64);
        doc.addFont('Roboto-Regular.ttf', 'Roboto', 'normal');
        doc.setFont('Roboto'); 

        doc.setFontSize(18);
        doc.text("TimeSheet Report", 14, 15);
        
        doc.setFontSize(10);
        doc.text(`Generated on: ${new Date().toLocaleString()}`, 14, 22);

        const tableColumn = ["Date", "Team member", "Projects", "Categories", "Description", "Time"];
        
        const tableRows = reportData.map(item => [
            new Date(item.date).toLocaleDateString(),
            item.memberName,
            item.projectName,
            item.categoryName,
            item.description,
            (item.time + item.overTime).toString()
        ]);

        autoTable(doc, {
            head: [tableColumn],
            body: tableRows,
            startY: 30,
            theme: 'striped',
            styles: { 
                font: 'Roboto',
                fontStyle: 'normal',
                fontSize: 10 
            },
            headStyles: { 
                fillColor: [243, 108, 33] 
            }
        });

        const finalY = (doc as any).lastAutoTable.finalY;
        doc.setFontSize(12);
        doc.text(`Report total: ${totalHours}h`, 14, finalY + 10);

        doc.save(`TimeSheet_Report_${new Date().toISOString().split('T')[0]}.pdf`);
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
                        <select value={filters.teamMemberId} onChange={(e) => setFilters({...filters, teamMemberId: e.target.value})}>
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
                    <button className="search-btn" onClick={handleSearch} disabled={isLoading}>
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
                    {reportData.length > 0 ? (
                        reportData.map(item => (
                            <tr key={item.id}>
                                <td>{new Date(item.date).toLocaleDateString()}</td>
                                <td>{item.memberName}</td>
                                <td>{item.projectName}</td>
                                <td>{item.categoryName}</td>
                                <td>{item.description}</td>
                                <td className="text-right">{item.time + item.overTime}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={6} style={{textAlign: 'center', padding: '30px', color: '#999'}}>
                                No activities found for the selected criteria.
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>

            <div className="reports-footer">
                <div className="export-actions">
                    <button className="export-btn" onClick={() => window.print()}>Print report</button>
                    <button className="export-btn" onClick={createPDF}>Create PDF</button>
                    <button className="export-btn" onClick={exportToExcel}>Export to excel</button>
                </div>
                <div className="report-total">
                    Report total: <span>{totalHours}</span>
                </div>
            </div>
        </div>
    );
};