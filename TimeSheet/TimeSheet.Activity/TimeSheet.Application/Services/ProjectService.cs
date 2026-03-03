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
using TimeSheet.Application.Common.Exceptions;
using FluentValidation;
using TimeSheet.Application.Validators;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Services
{
    public class ProjectService : BaseService, IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectLeadRepository _projectLeadRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IValidator<ProjectRequestDTO> _validator;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IProjectLeadRepository projectLeadRepository,IClientRepository clientRepository, IMemberRepository memberRepository, IValidator<ProjectRequestDTO> validator, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _projectLeadRepository = projectLeadRepository;
            _clientRepository = clientRepository;
            _memberRepository = memberRepository;
            _validator = validator;
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
            await ValidateAsync(_validator, projectRequestDTO);

            var client = await _clientRepository.FindClientByIdAsync(projectRequestDTO.ClientId);
            if (client == null) throw new NotFoundException($"Client with ID {projectRequestDTO.ClientId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(projectRequestDTO.CurrentLead);
            if (member == null) throw new NotFoundException($"Member with ID {projectRequestDTO.CurrentLead} not found.");

            var project = _mapper.Map<Project>(projectRequestDTO);

            if (projectRequestDTO.CurrentLead.HasValue)
            {
                var memberL = await _memberRepository.FindMemberByIdAsync(projectRequestDTO.CurrentLead.Value);
                if (memberL == null) throw new NotFoundException("Member not found.");

                project.CurrentLeadId = member.Id;
                await _projectLeadRepository.AssignLeadAsync(project.Id, member.Id);
            }

            var createdProject = await _projectRepository.AddProjectAsync(project);
            return _mapper.Map<ProjectDTO>(createdProject);
        }

        public async Task<ProjectDTO> UpdateProjectAsync(Guid id, ProjectRequestDTO projectRequestDTO)
        {
            await ValidateAsync(_validator, projectRequestDTO);

            var existingProject = await _projectRepository.FindProjectByIdAsync(id);
            if (existingProject == null) throw new NotFoundException($"Project with ID {id} not found.");

            if (existingProject.CurrentLeadId != projectRequestDTO.CurrentLead)
            {
                var leadExists = await _memberRepository.FindMemberByIdAsync(projectRequestDTO.CurrentLead);
                if (leadExists == null) throw new NotFoundException("New Lead member not found.");
            }

            _mapper.Map(projectRequestDTO, existingProject);
            var updatedProject = await _projectRepository.UpdateProjectAsync(existingProject);
            return _mapper.Map<ProjectDTO>(updatedProject);
        }

        public async Task ClearCurrentLeadAsync(Guid projectId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new NotFoundException("Project not found.");

            project.CurrentLeadId = null; 
            await _projectRepository.UpdateProjectAsync(project);
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            var existingProject = await _projectRepository.FindProjectByIdAsync(id);
            if (existingProject == null) throw new NotFoundException($"Project with ID {id} not found.");

            await _projectRepository.DeleteProjectAsync(existingProject);
        }
    }
}