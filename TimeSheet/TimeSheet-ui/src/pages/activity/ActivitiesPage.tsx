import React, { useEffect, useState } from 'react';
import { activityService } from '../../services/activityService';
import { ActivityDTO, ActivityRequestDTO, ActivitySummaryDTO } from '../../types/activity';
import './ActivitiesPage.css';
import { DateView } from './DateView';
import { ProjectDTO } from '../../types/project';
import { CategoryDTO } from '../../types/category';
import { projectService } from '../../services/projectService';
import { projectMemberService } from '../../services/projectMemberService';
import { categoryService } from '../../services/categoryService';

export const ActivitiesPage: React.FC = () => {

    const userJson = localStorage.getItem('user');
    const currentUser = userJson ? JSON.parse(userJson) : null;

    const [currentMonth, setCurrentMonth] = useState<Date>(new Date());
    const [activitiesSummary, setActivitiesSummary] = useState<ActivitySummaryDTO | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    const [projects, setProjects] = useState<ProjectDTO[]>([]);
    const [categories, setCategories] = useState<CategoryDTO[]>([]);
    
    const today = new Date();

    const [view, setView] = useState<'monthly' | 'daily'>('monthly');
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);

    const formatDateToDateOnly = (date: Date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`; 
    };

    useEffect(() => {
        const loadInitialData = async () => {

            if (!currentUser?.id) return;

            setIsLoading(true);
            try {
                const [projData, catData] = await Promise.all([
                    projectMemberService.getProjectsByMember(currentUser.id), 
                    categoryService.getAllCategories()
                ]);
                setProjects(projData);
                setCategories(catData);
            } catch (error) {
                console.error("Error while loading categories/projects:", error);
            } finally {
                setIsLoading(false);
            }
        };
        loadInitialData();
    }, []); 

    useEffect(() => {
        fetchMonthData();
    }, [currentMonth]);

    
    const handleDateChange = (newDate: Date) => {
        setSelectedDate(newDate);
    };

        const fetchMonthData = async () => {
        setIsLoading(true);
        try {
            const firstDayDate = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), 1);
            const lastDayDate = new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 0);
            
            const startDate = formatDateToDateOnly(firstDayDate);
            const endDate = formatDateToDateOnly(lastDayDate);
            
            
            const data = await activityService.getActivitiesByDate(startDate, endDate);
            setActivitiesSummary(data);
        } catch (error) {
            console.error("Error fetching activities:", error);
        } finally {
            setIsLoading(false);
        }
    };

    const handleSaveActivities = async (activitiesToSave: ActivityRequestDTO[]) => {
        if (!selectedDate) return;

        const dateStr = formatDateToDateOnly(selectedDate);

        const newReqTime = activitiesToSave.reduce((acc, curr) => acc + Number(curr.time), 0);
        const newReqOvertime = activitiesToSave.reduce((acc, curr) => acc + Number(curr.overTime), 0);

        const existingActivitiesForDay = activitiesSummary?.activities.filter(a => 
            a.date.startsWith(dateStr) && !activitiesToSave.some(newAct => (newAct as any).id === a.id)
        ) || [];

        const existingTime = existingActivitiesForDay.reduce((acc, curr) => acc + Number(curr.time), 0);
        const existingOvertime = existingActivitiesForDay.reduce((acc, curr) => acc + Number(curr.overTime), 0);

        const totalTime = existingTime + newReqTime;
        const totalOvertime = existingOvertime + newReqOvertime;
        const totalDayHours = totalTime + totalOvertime;

        if (totalTime > 8) {
            alert(`Standard working hours for this day cannot exceed 8 hours (total would be: ${totalTime}h).`);
            return;
        }

        if (totalOvertime > 4) {
            alert(`Overtime for this day cannot exceed 4 hours (total would be: ${totalOvertime}h).`);
            return;
        }

        if (totalDayHours > 12) {
            alert(`Total daily working hours cannot exceed 12 hours (total would be: ${totalDayHours}h).`);
            return;
        }

        setIsLoading(true);
        try {
            const savePromises = activitiesToSave.map(activity => 
                activityService.createActivity(activity)
            );

            await Promise.all(savePromises);
            alert("Activities successfully saved!");
            
            setView('monthly');
            fetchMonthData(); 
        } catch (error) {
            console.error("Failed to save activities:", error);
            alert("Something went wrong while saving. Please check the console.");
        } finally {
            setIsLoading(false);
        }
    };

    if (view === 'daily' && selectedDate) {
        return (
            <DateView 
                date={selectedDate}
                onDateChange={handleDateChange}
                projects={projects}
                categories={categories}
                existingActivities={activitiesSummary?.activities.filter(a => a.date.startsWith(formatDateToDateOnly(selectedDate))) || []}
                onBack={() => setView('monthly')}
                onSave={handleSaveActivities}            />
        );
    }

    const handleDayClick = (date: Date) => {
        setSelectedDate(date);
        setView('daily');
    };

    const getDaysInMonth = (date: Date) => {
        const year = date.getFullYear();
        const month = date.getMonth();
        const days = [];
        
        const firstDayOfMonth = new Date(year, month, 1).getDay();
        const startOffset = firstDayOfMonth === 0 ? 6 : firstDayOfMonth - 1;

        for (let i = 0; i < startOffset; i++) {
            days.push(null);
        }

        const numDays = new Date(year, month + 1, 0).getDate();
        for (let i = 1; i <= numDays; i++) {
            days.push(new Date(year, month, i));
        }
        return days;
    };

    const isCurrentWeek = (date: Date) => {
        const curr = new Date();
        const firstDay = curr.getDate() - (curr.getDay() === 0 ? 6 : curr.getDay() - 1);
        const lastDay = firstDay + 6;

        const first = new Date(curr.setDate(firstDay)).setHours(0,0,0,0);
        const last = new Date(curr.setDate(lastDay)).setHours(23,59,59,999);
        const check = date.getTime();

        return check >= first && check <= last;
    };

    const handlePrevMonth = () => {
        setCurrentMonth(new Date(currentMonth.setMonth(currentMonth.getMonth() - 1)));
    };

    const handleNextMonth = () => {
        const next = new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 1);
        if (next <= today) {
            setCurrentMonth(next);
        }
    };

    const getHoursForDate = (date: Date) => {
        const dateStr = formatDateToDateOnly(date);
        
        const dayActivities = activitiesSummary?.activities.filter(a => {
            return a.date.startsWith(dateStr);
        });
        
        return dayActivities?.reduce((acc, curr) => acc + curr.time + curr.overTime, 0) || 0;
    };

    const isEditableWorkDay = (date: Date) => {
        const curr = new Date();

        const firstDay = curr.getDate() - (curr.getDay() === 0 ? 6 : curr.getDay() - 1);
        const lastDay = firstDay + 6;

        const first = new Date(curr.setDate(firstDay)).setHours(0,0,0,0);
        const last = new Date(curr.setDate(lastDay)).setHours(23,59,59,999);
        const check = date.getTime();

        const isInCurrentWeek = check >= first && check <= last;
        
        const dayOfWeek = date.getDay();
        const isWorkDay = dayOfWeek >= 1 && dayOfWeek <= 5;

        return isInCurrentWeek && isWorkDay;
    };

    const monthName = currentMonth.toLocaleString('default', { month: 'long', year: 'numeric' });

    return (
        <div className="page-container">
            <div className="page-header">
                <h1 className="page-title">TimeSheet</h1>
            </div>

            <div className="calendar-nav">
                <button className="nav-btn" onClick={handlePrevMonth}>&lt; previous month</button>
                <span className="current-month-display">{monthName}</span>
                <button 
                    className={`nav-btn ${new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 1) > today ? 'disabled' : ''}`} 
                    onClick={handleNextMonth}
                >
                    next month &gt;
                </button>
            </div>

            <div className="calendar-grid">
                <div className="calendar-day-header">MONDAY</div>
                <div className="calendar-day-header">TUESDAY</div>
                <div className="calendar-day-header">WEDNESDAY</div>
                <div className="calendar-day-header">THURSDAY</div>
                <div className="calendar-day-header">FRIDAY</div>
                <div className="calendar-day-header">SATURDAY</div>
                <div className="calendar-day-header">SUNDAY</div>

                    {getDaysInMonth(currentMonth).map((date, index) => (
                        <div 
                            key={index} 
                            onClick={() => date && handleDayClick(date)}
                            className={`calendar-cell ${!date ? 'empty' : ''} ${date && isEditableWorkDay(date) ? 'editable-week' : ''}`}
                        >
                            {date && (
                                <>
                                    <span className="day-number">{date.getDate()}.</span>
                                    <div className="hours-display">
                                        Hours: <strong>{getHoursForDate(date)}</strong>
                                    </div>
                                </>
                            )}
                        </div>
                    ))}
            </div>

            <div className="calendar-footer">
                <div className="total-hours">
                    Total hours: <span>{activitiesSummary?.totalHours || 0}</span>
                </div>
            </div>
        </div>
    );
};