using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Activity;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Domain.Entities.Enums;
using TimeSheet.Application.Common.Exceptions;
using FluentValidation;
using TimeSheet.Application.Validators;
using TimeSheet.Application.Common.Extensions;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectRepository _projectRepository; 
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<ActivityRequestDTO> _validator;
        private readonly IMapper _mapper;

        public ActivityService(IActivityRepository activityRepository, ICurrentUserService currentUserService, IProjectRepository projectRepository, ICategoryRepository categoryRepository, IValidator<ActivityRequestDTO> validator, IMapper mapper)
        {
            _activityRepository = activityRepository;
            _currentUserService = currentUserService;
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ActivityDTO> GetActivityByIdAsync(Guid id)
        {
            var activity = await _activityRepository.FindActivityByIdAsync(id);
            if (activity == null) throw new NotFoundException($"Activity with ID {id} not found.");

            return _mapper.Map<ActivityDTO>(activity);
        }

        public async Task<ActivitySummaryDTO> GetActivitiesByDateAsync(DateOnly startDate, DateOnly endDate)
        {
            var activities = await _activityRepository.FindActivitiesByDateAsync(startDate, endDate);

            var activityDTOs = _mapper.Map<IEnumerable<ActivityDTO>>(activities);

            decimal total = activityDTOs.Sum(a => a.Time + a.OverTime);

            return new ActivitySummaryDTO
            {
                Activities = activityDTOs,
                TotalHours = total
            };
        }

        public async Task<IEnumerable<ActivityDTO>> GetAllActivitiesAsync()
        {
            var activities = await _activityRepository.FindAllActivitiesAsync();
            return _mapper.Map<IEnumerable<ActivityDTO>>(activities);
        }

        public async Task<ActivityDTO> CreateActivityAsync(ActivityRequestDTO activityRequestDTO)
        {
            await _validator.ValidateAndThrowAsync(activityRequestDTO);

            var project = await _projectRepository.FindProjectByIdAsync(activityRequestDTO.ProjectId);
            if (project == null) throw new NotFoundException($"Project with ID {activityRequestDTO.ProjectId} not found.");

            var category = await _categoryRepository.FindCategoryByIdAsync(activityRequestDTO.CategoryId);
            if (category == null) throw new NotFoundException($"Category with ID {activityRequestDTO.CategoryId} not found.");

            if (project.Status == ProjectStatus.INACTIVE) 
                throw new BadRequestException("Cannot log time on an inactive project.");

            var activity = _mapper.Map<Activity>(activityRequestDTO);
            activity.MemberId = _currentUserService.UserId;

            var createdActivity = await _activityRepository.AddActivityAsync(activity);
            return _mapper.Map<ActivityDTO>(createdActivity);
        }

        public async Task<IEnumerable<ActivityDTO>> SearchActivitiesAsync(
            Guid? memberId,
            Guid? clientId,
            Guid? projectId,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                throw new BadRequestException("Both start and end dates must be selected for the search.");
            }

            if (endDate.Value > startDate.Value.AddMonths(6))
            {
                throw new BadRequestException("The search range cannot exceed 6 months.");
            }

            if (endDate.Value < startDate.Value)
            {
                throw new TimeSheetValidationException("End date cannot be before start date.");
            }

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

