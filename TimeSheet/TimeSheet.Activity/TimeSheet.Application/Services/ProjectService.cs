using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Project;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Application.Common.DTOs;

namespace TimeSheet.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectLeadRepository _projectLeadRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IProjectLeadRepository projectLeadRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _projectLeadRepository = projectLeadRepository;
            _mapper = mapper;
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(Guid id)
        {
            var project = await _projectRepository.FindProjectByIdAsync(id);
            if (project == null) throw new KeyNotFoundException($"Project with ID {id} not found.");

            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.FindAllProjectsAsync();
            return _mapper.Map<IEnumerable<ProjectDTO>>(projects);
        }

        public async Task<PagedList<ProjectDTO>> GetAllProjectsPagedAsync(PagedListDTO pagedListDTO)
        {
            var pagedProjects = await _projectRepository.FindAllProjectsPagedAsync(
                pagedListDTO.PageNumber, pagedListDTO.PageSize, pagedListDTO.SearchTerm, pagedListDTO.FirstLetter, pagedListDTO.Order);

            var dtos = _mapper.Map<PagedList<ProjectDTO>>(pagedProjects);

            return dtos;
        }

        public async Task<ProjectDTO> CreateProjectAsync(ProjectRequestDTO projectRequestDTO)
        {
            var project = _mapper.Map<Project>(projectRequestDTO);
            project.Id = Guid.NewGuid();

            await _projectRepository.AddProjectAsync(project);
            await _projectLeadRepository.AssignLeadAsync(project.Id, projectRequestDTO.CurrentLead);

            var createdProject = await _projectRepository.FindProjectByIdAsync(project.Id);
            return _mapper.Map<ProjectDTO>(createdProject);
        }

        public async Task<ProjectDTO> UpdateProjectAsync(Guid id, ProjectRequestDTO projectRequestDTO)
        {
            var existingProject = await _projectRepository.FindProjectByIdAsync(id);
            if (existingProject == null) throw new KeyNotFoundException($"Project with ID {id} not found.");

            _mapper.Map(projectRequestDTO, existingProject);
            await _projectRepository.UpdateProjectAsync(existingProject);

            var updatedProject = await _projectRepository.FindProjectByIdAsync(id);
            return _mapper.Map<ProjectDTO>(updatedProject);
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            var existingProject = await _projectRepository.FindProjectByIdAsync(id);
            if (existingProject == null) throw new KeyNotFoundException($"Project with ID {id} not found.");

            await _projectRepository.DeleteProjectAsync(existingProject);
        }
    }
}