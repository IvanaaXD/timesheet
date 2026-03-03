using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Activity;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.Exceptions;
using TimeSheet.Domain.Entities.Enums;

namespace TimeSheet.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectRepository _projectRepository; 
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ActivityService(IActivityRepository activityRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _activityRepository = activityRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ActivityDTO> GetActivityByIdAsync(Guid id)
        {
            var activity = await _activityRepository.FindActivityByIdAsync(id);
            if (activity == null) throw new NotFoundException($"Activity with ID {id} not found.");

            return _mapper.Map<ActivityDTO>(activity);
        }

        public async Task<IEnumerable<ActivityDTO>> GetActivitiesByDateAsync(DateOnly date)
        {
            var activities = await _activityRepository.FindActivitiesByDateAsync(date);
            return _mapper.Map<IEnumerable<ActivityDTO>>(activities);
        }

        public async Task<IEnumerable<ActivityDTO>> GetAllActivitiesAsync()
        {
            var activities = await _activityRepository.FindAllActivitiesAsync();
            return _mapper.Map<IEnumerable<ActivityDTO>>(activities);
        }

        public async Task<ActivityDTO> CreateActivityAsync(ActivityRequestDTO activityRequestDTO)
        {
            var project = await _projectRepository.FindProjectByIdAsync(activityRequestDTO.ProjectId);
            if (project == null) throw new NotFoundException($"Project with ID {activityRequestDTO.ProjectId} not found.");

            var category = await _categoryRepository.FindCategoryByIdAsync(activityRequestDTO.CategoryId);
            if (category == null) throw new NotFoundException($"Category with ID {activityRequestDTO.CategoryId} not found.");

            if (activityRequestDTO.Time < 0 || activityRequestDTO.OverTime < 0)
                throw new ValidationException("Hours cannot be negative.");

            if (activityRequestDTO.Time + activityRequestDTO.OverTime > 24)
                throw new ValidationException("Total hours in a day cannot exceed 24.");

            if (project.Status == ProjectStatus.INACTIVE) 
                throw new BadRequestException("Cannot log time on an inactive project.");

            var activity = _mapper.Map<Activity>(activityRequestDTO);
            activity.Id = Guid.NewGuid();
            activity.MemberId = _currentUserService.UserId;

            await _activityRepository.AddActivityAsync(activity);
            return _mapper.Map<ActivityDTO>(activity);
        }

        public async Task<IEnumerable<ActivityDTO>> SearchActivitiesAsync(
            Guid? memberId,
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var activities = await _activityRepository.SearchActivitiesAsync(
                memberId: memberId,
                clientId: clientId,
                projectId: projectId,
                categoryId: categoryId,
                startDate: startDate,
                endDate: endDate
            );
            return _mapper.Map<IEnumerable<ActivityDTO>>(activities);  
        }
    }
}

