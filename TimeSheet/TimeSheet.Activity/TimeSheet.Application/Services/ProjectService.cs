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
using TimeSheet.Application.Exceptions;

namespace TimeSheet.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectLeadRepository _projectLeadRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IProjectLeadRepository projectLeadRepository,IClientRepository clientRepository, IMemberRepository memberRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _projectLeadRepository = projectLeadRepository;
            _clientRepository = clientRepository;
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(Guid id)
        {
            var project = await _projectRepository.FindProjectByIdAsync(id);
            if (project == null) throw new NotFoundException($"Project with ID {id} not found.");

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
            var client = await _clientRepository.FindClientByIdAsync(projectRequestDTO.ClientId);
            if (client == null) throw new NotFoundException($"Client with ID {projectRequestDTO.ClientId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(projectRequestDTO.CurrentLead);
            if (member == null) throw new NotFoundException($"Member with ID {projectRequestDTO.CurrentLead} not found.");

            // provjera imena projekta?

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
            if (existingProject == null) throw new NotFoundException($"Project with ID {id} not found.");

            if (existingProject.CurrentLeadId != projectRequestDTO.CurrentLead)
            {
                var leadExists = await _memberRepository.FindMemberByIdAsync(projectRequestDTO.CurrentLead);
                if (leadExists == null) throw new NotFoundException("New Lead member not found.");
            }

            _mapper.Map(projectRequestDTO, existingProject);
            await _projectRepository.UpdateProjectAsync(existingProject);

            var updatedProject = await _projectRepository.FindProjectByIdAsync(id);
            return _mapper.Map<ProjectDTO>(updatedProject);
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            var existingProject = await _projectRepository.FindProjectByIdAsync(id);
            if (existingProject == null) throw new NotFoundException($"Project with ID {id} not found.");

            await _projectRepository.DeleteProjectAsync(existingProject);
        }
    }
}