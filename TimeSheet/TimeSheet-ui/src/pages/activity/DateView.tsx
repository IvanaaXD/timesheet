import React, { useState, useEffect, useMemo } from 'react';
import { ActivityRequestDTO, ActivityDTO } from '../../types/activity';
import { ProjectDTO } from '../../types/project';
import { CategoryDTO } from '../../types/category';
import './DateView.css';

interface LocalActivityRow extends ActivityRequestDTO {
    isExisting?: boolean;
}

interface DateViewProps {
    date: Date;
    existingActivities: ActivityDTO[];
    projects: ProjectDTO[];
    categories: CategoryDTO[];
    onBack: () => void;
    onSave: (activities: ActivityRequestDTO[]) => void;
    onDateChange: (newDate: Date) => void; 
}

export const DateView: React.FC<DateViewProps> = ({ 
    date, existingActivities, projects, categories, onBack, onSave, onDateChange 
}) => {
    const [rows, setRows] = useState<LocalActivityRow[]>([]);
    
    const localFormat = (d: Date) => {
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    const selectedDateStr = localFormat(date);

    const getWeekNumber = (d: Date) => {
        const target = new Date(d.valueOf());
        const dayNr = (d.getDay() + 6) % 7;
        target.setDate(target.getDate() - dayNr + 3);
        const firstThursday = target.valueOf();
        target.setMonth(0, 1);
        if (target.getDay() !== 4) {
            target.setMonth(0, 1 + ((4 - target.getDay()) + 7) % 7);
        }
        return 1 + Math.ceil((firstThursday - target.valueOf()) / 604800000);
    };

    const weekDays = useMemo(() => {
        const days = [];
        const startOfWeek = new Date(date);
        const day = startOfWeek.getDay();
        const diff = startOfWeek.getDate() - (day === 0 ? 6 : day - 1); 
        startOfWeek.setDate(diff);

        for (let i = 0; i < 7; i++) {
            const d = new Date(startOfWeek);
            d.setDate(startOfWeek.getDate() + i);
            days.push(d);
        }
        return days;
    }, [date]);

    const weekRangeInfo = useMemo(() => {
        const start = weekDays[0];
        const end = weekDays[6];
        const options: Intl.DateTimeFormatOptions = { month: 'long', day: '2-digit' };
        const startStr = start.toLocaleDateString('en-US', options);
        const endStr = end.toLocaleDateString('en-US', options);
        const year = start.getFullYear();
        const weekNum = getWeekNumber(start);

        return `${startStr} - ${endStr}, ${year} (week ${weekNum})`;
    }, [weekDays]);

    useEffect(() => {
        const dbRows: LocalActivityRow[] = existingActivities.map(a => ({
            description: a.description,
            date: a.date,
            time: a.time,
            overTime: a.overTime,
            projectId: projects.find(p => p.name === a.projectName)?.id || '',
            categoryId: categories.find(c => c.name === a.categoryName)?.id || '',
            isExisting: true 
        }));

        if (dbRows.length === 0) {
            setRows(Array(5).fill(null).map(() => ({
                description: '', date: selectedDateStr, time: 0, overTime: 0, projectId: '', categoryId: '', isExisting: false
            })));
        } else {
            setRows(dbRows);
        }
    }, [existingActivities, projects, categories, selectedDateStr]);

    const isEditable = () => {
        const curr = new Date();
        const day = curr.getDay();
        const diff = curr.getDate() - (day === 0 ? 6 : day - 1);
        const startOfWeek = new Date(curr.setDate(diff));
        startOfWeek.setHours(0, 0, 0, 0);
        const endOfWeek = new Date(startOfWeek);
        endOfWeek.setDate(startOfWeek.getDate() + 4); 
        return date >= startOfWeek && date <= endOfWeek && date.getDay() >= 1 && date.getDay() <= 5;
    };

    const showSubmit = () => {
        const now = new Date(); 
        const submissionStartTime = new Date(date);
        submissionStartTime.setHours(15, 0, 0, 0);
        const day = date.getDay();
        const diffToFriday = (day === 0 ? -2 : 5 - day); 
        const fridayDeadline = new Date(date);
        fridayDeadline.setDate(date.getDate() + diffToFriday);
        fridayDeadline.setHours(23, 59, 59, 999);
        return now >= submissionStartTime && now <= fridayDeadline;
    };

    const handleWeekNav = (direction: number) => {
        const newDate = new Date(date);
        newDate.setDate(date.getDate() + direction);
        onDateChange(newDate);
    };

    const totalHours = rows.reduce((sum, row) => sum + Number(row.time || 0) + Number(row.overTime || 0), 0);

    return (
        <div className="date-view-container">
            <div className="week-range-display">
                {weekRangeInfo}
            </div>

            <div className="week-navigation-header">
                <button className="nav-arrow" onClick={() => handleWeekNav(-7)}>‹</button>
                <div className="days-list">
                    {weekDays.map((d, i) => {
                        const isSelected = localFormat(d) === selectedDateStr;
                        return (
                            <div 
                                key={i} 
                                className={`day-item ${isSelected ? 'selected' : ''}`}
                                onClick={() => onDateChange(d)}
                            >
                                <span className="day-name">{d.toLocaleDateString('en-US', { weekday: 'short' }).toUpperCase()}</span>
                                <span className="day-date">{d.getDate()}</span>
                            </div>
                        );
                    })}
                </div>
                <button className="nav-arrow" onClick={() => handleWeekNav(7)}>›</button>
            </div>

            <table className="activities-table">
                <thead>
                    <tr>
                        <th>Client *</th><th>Project *</th><th>Category *</th><th>Description</th><th>Time *</th><th>Overtime</th>
                    </tr>
                </thead>
                <tbody>
                    {rows.map((row, index) => {
                        const activeProject = projects.find(p => p.id === row.projectId);
                        const isDisabled = row.isExisting || !isEditable();
                        const availableProjects = projects.filter(p => 
                            p.id === row.projectId || !rows.some(r => r.projectId === p.id)
                        );

                        return (
                            <tr key={index} className={row.isExisting ? 'row-locked' : ''}>
                                <td><input value={activeProject?.clientName || 'Choose project'} disabled className="readonly-input" /></td>
                                <td>
                                    <select 
                                        value={row.projectId} 
                                        onChange={(e) => setRows(rows.map((r, i) => i === index ? {...r, projectId: e.target.value} : r))} 
                                        disabled={isDisabled}
                                    >
                                        <option value="">Choose project</option>
                                        {availableProjects.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                                    </select>
                                </td>
                                <td>
                                    <select value={row.categoryId} onChange={(e) => setRows(rows.map((r, i) => i === index ? {...r, categoryId: e.target.value} : r))} disabled={isDisabled}>
                                        <option value="">Choose category</option>
                                        {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                                    </select>
                                </td>
                                <td><input value={row.description} onChange={(e) => setRows(rows.map((r, i) => i === index ? {...r, description: e.target.value} : r))} disabled={isDisabled}/></td>
                                <td>
                                    <input 
                                        type="number" 
                                        value={row.time || 0} 
                                        onChange={(e) => {
                                            const val = e.target.value === '' ? 0 : parseFloat(e.target.value);
                                            setRows(rows.map((r, i) => i === index ? {...r, time: isNaN(val) ? 0 : val} : r))
                                        }} 
                                        disabled={isDisabled} 
                                        className="small-input"
                                    />
                                </td>
                                <td>
                                    <input 
                                        type="number" 
                                        value={row.overTime || 0} 
                                        onChange={(e) => {
                                            const val = e.target.value === '' ? 0 : parseFloat(e.target.value);
                                            setRows(rows.map((r, i) => i === index ? {...r, overTime: isNaN(val) ? 0 : val} : r))
                                        }} 
                                        disabled={isDisabled} 
                                        className="small-input"
                                    />
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>

            <div className="table-actions">
                {isEditable() && (
                    <button 
                        className="add-row-link" 
                        onClick={() => setRows([...rows, { description: '', date: selectedDateStr, time: 0, overTime: 0, projectId: '', categoryId: '', isExisting: false }])}
                    >
                        + add more rows
                    </button>
                )}
            </div>

            <div className="view-footer">
                <div className="footer-left">
                    <button className="back-to-monthly" onClick={onBack}>back to monthly view</button>
                </div>
                
                <div className="footer-middle">
                    {showSubmit() && isEditable() && (
                        <button 
                            className="submit-btn" 
                            onClick={() => {
                                const newActivities = rows.filter(r => !r.isExisting && r.projectId);
                                if (newActivities.length > 0) {
                                    onSave(newActivities);
                                } else {
                                    alert("No new activities to submit.");
                                }
                            }}
                        >
                            Submit Activities
                        </button>
                    )}
                </div>

                <div className="footer-right">
                    <div className="total-display">Total hours: <strong>{totalHours}</strong></div>
                </div>
            </div>
        </div>
    );
};