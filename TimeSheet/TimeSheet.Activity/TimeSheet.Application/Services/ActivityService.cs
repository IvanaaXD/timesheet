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

namespace TimeSheet.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
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
            var currentUserId = _currentUserService.UserId;
            var activity = _mapper.Map<Activity>(activityRequestDTO);

            activity.Id = Guid.NewGuid();
            activity.MemberId = currentUserId;
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
                memberId: _currentUserService.UserId,
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

