using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.DTOs.Project;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.Common.Exceptions;

namespace TimeSheet.Application.Services
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectMemberService(IProjectMemberRepository projectMemberRepository, IMemberRepository memberRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _projectMemberRepository = projectMemberRepository;
            _memberRepository = memberRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task AddMemberToProjectAsync(Guid projectId, Guid memberId, bool isLead)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new NotFoundException($"Project with ID {projectId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(memberId);
            if (member == null) throw new NotFoundException($"Member with ID {memberId} not found.");

            if (member.Status == Domain.Entities.Enums.MemberStatus.INACTIVE)
                throw new BadRequestException("Cannot assign an inactive member to a project.");

            var alreadyOnProject = await _projectMemberRepository.IsMemberOnProjectAsync(memberId, projectId);
            if (alreadyOnProject)
                throw new ConflictException("Member is already assigned to this project.");

            await _projectMemberRepository.AddMemberToProjectAsync(projectId, memberId, isLead);

            if (isLead)
            {
                project.CurrentLeadId = memberId;
                await _projectRepository.UpdateProjectAsync(project);
            }
        }

        public async Task AssignLeadAsync(Guid projectId, Guid memberId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new NotFoundException($"Project with ID {projectId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(memberId);
            if (member == null) throw new NotFoundException($"Member with ID {memberId} not found.");

            if (project.CurrentLeadId == memberId)
                throw new ConflictException("Member is already the current lead for this project.");

            project.CurrentLeadId = memberId;
            await _projectRepository.UpdateProjectAsync(project);

            var isOnProject = await _projectMemberRepository.IsMemberOnProjectAsync(memberId, projectId);

            if (isOnProject)
            {
                await _projectMemberRepository.ChangeMemberRoleOnProjectAsync(projectId, memberId, isLead: true);
            }
            else
            {
                await _projectMemberRepository.AddMemberToProjectAsync(projectId, memberId, isLead: true);
            }
        }

        public async Task RemoveLeadAsync(Guid projectId, Guid memberId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new NotFoundException($"Project with ID {projectId} not found.");

            if (project.CurrentLeadId != memberId)
                throw new BadRequestException($"Member with ID {memberId} is not the current lead of this project.");

            project.CurrentLeadId = null;
            await _projectRepository.UpdateProjectAsync(project);

            await _projectMemberRepository.ChangeMemberRoleOnProjectAsync(projectId, memberId, isLead: false);
        }

        public async Task RemoveMemberFromProjectAsync(Guid projectId, Guid memberId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new NotFoundException($"Project with ID {projectId} not found.");

            var isOnProject = await _projectMemberRepository.IsMemberOnProjectAsync(memberId, projectId);
            if (!isOnProject) throw new NotFoundException("Member is not assigned to this project.");

            if (project.CurrentLeadId == memberId)
            {
                project.CurrentLeadId = null;
                await _projectRepository.UpdateProjectAsync(project);
            }

            await _projectMemberRepository.RemoveMemberFromProjectAsync(projectId, memberId);
        }

        public async Task<IEnumerable<ProjectDTO>> GetProjectsByMemberAsync(Guid memberId)
        {
            var projects = await _projectMemberRepository.FindProjectsByMemberAsync(memberId);
            return _mapper.Map<IEnumerable<ProjectDTO>>(projects);
        }

        public async Task<IEnumerable<MemberDTO>> GetMembersByProjectAsync(Guid projectId)
        {
            var members = await _projectMemberRepository.FindMembersByProjectAsync(projectId);
            return _mapper.Map<IEnumerable<MemberDTO>>(members);
        }

        public async Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new NotFoundException($"Project with ID {projectId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(memberId);
            if (member == null) throw new NotFoundException($"Member with ID {memberId} not found.");

            return await _projectMemberRepository.IsMemberLeadOfProjectAsync(memberId, projectId);
        }
    }
}
