import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { MainLayout } from './layouts/MainLayout';

import { LoginPage } from './pages/login/LoginPage'; 
import { CategoriesPage } from './pages/category/CategoriesPage'; 
import { ClientsPage } from './pages/client/ClientsPage'; 
import { ProjectsPage } from './pages/project/ProjectsPage'; 
import { MembersPage } from './pages/member/MembersPage'; 
import { ActivitiesPage } from './pages/activity/ActivitiesPage'; 
import { ReportsPage } from './pages/report/ReportsPage'; 

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route element={<MainLayout />}>
          <Route path="/" element={<ActivitiesPage />} />
          <Route path="/clients" element={<ClientsPage />} />
          <Route path="/projects" element={<ProjectsPage />} />
          <Route path="/categories" element={<CategoriesPage />} />
          <Route path="/team-members" element={<MembersPage />} />
          <Route path="/reports" element={<ReportsPage />} />
        </Route>

        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </Router>
  );
}

export default App;