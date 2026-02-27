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

namespace TimeSheet.Application.Services
{
    public class ProjectLeadService : IProjectLeadService
    {
        private readonly IProjectLeadRepository _projectLeadRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectLeadService(IProjectLeadRepository projectLeadRepository, IMemberRepository memberRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _projectLeadRepository = projectLeadRepository;
            _memberRepository = memberRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task AssignLeadAsync(Guid projectId, Guid memberId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new KeyNotFoundException($"Project with ID {projectId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(memberId);
            if (member == null) throw new KeyNotFoundException($"Member with ID {memberId} not found.");

            var isLead = await _projectLeadRepository.IsMemberLeadOfProjectAsync(memberId, projectId);
            if (isLead) return;

            await _projectLeadRepository.AssignLeadAsync(projectId, memberId);
        }

        public async Task RemoveLeadAsync(Guid projectId, Guid memberId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new KeyNotFoundException($"Project with ID {projectId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(memberId);
            if (member == null) throw new KeyNotFoundException($"Member with ID {memberId} not found.");

            var isLead = await _projectLeadRepository.IsMemberLeadOfProjectAsync(memberId, projectId);
            if (!isLead) throw new KeyNotFoundException($"Member with ID {memberId} does not lead the project with ID {projectId}");

            await _projectLeadRepository.RemoveLeadAsync(projectId, memberId);
        }

        public async Task<IEnumerable<ProjectDTO>> GetProjectsByLeadAsync(Guid memberId)
        {
            var projects = await _projectLeadRepository.FindProjectsByLeadAsync(memberId);
            return _mapper.Map<IEnumerable<ProjectDTO>>(projects);
        }

        public async Task<IEnumerable<MemberDTO>> GetLeadsByProjectAsync(Guid projectId)
        {
            var members = await _projectLeadRepository.FindLeadsByProjectAsync(projectId);
            return _mapper.Map<IEnumerable<MemberDTO>>(members);
        }

        public async Task<bool> IsMemberLeadOfProjectAsync(Guid memberId, Guid projectId)
        {
            var project = await _projectRepository.FindProjectByIdAsync(projectId);
            if (project == null) throw new KeyNotFoundException($"Project with ID {projectId} not found.");

            var member = await _memberRepository.FindMemberByIdAsync(memberId);
            if (member == null) throw new KeyNotFoundException($"Member with ID {memberId} not found.");

            return await _projectLeadRepository.IsMemberLeadOfProjectAsync(memberId, projectId);
        }
    }
}
