import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { MainLayout } from './layouts/MainLayout';

// 1. Увозимо праву страницу коју смо направили!
import { LoginPage } from './pages/login/LoginPage'; 
import { CategoriesPage } from './pages/category/CategoriesPage'; 
import { ClientsPage } from './pages/client/ClientsPage'; 
import { ProjectsPage } from './pages/project/ProjectsPage'; 
import { MembersPage } from './pages/member/MembersPage'; 


// Остале лажне странице остају док и њих не направимо
const TimeSheetPage = () => <h2>TimeSheet calendar</h2>;
const ReportsPage = () => <h2>Reports</h2>;

function App() {
  return (
    <Router>
      <Routes>
        {/* Јавна рута за логовањe користи праву компоненту */}
        <Route path="/login" element={<LoginPage />} />

        {/* Заштићене руте */}
        <Route element={<MainLayout />}>
          <Route path="/" element={<TimeSheetPage />} />
          <Route path="/clients" element={<ClientsPage />} />
          <Route path="/projects" element={<ProjectsPage />} />
          <Route path="/categories" element={<CategoriesPage />} />
          <Route path="/team-members" element={<MembersPage />} />
          <Route path="/reports" element={<ReportsPage />} />
        </Route>

        {/* Fallback за непостојеће руте */}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </Router>
  );
}

export default App;