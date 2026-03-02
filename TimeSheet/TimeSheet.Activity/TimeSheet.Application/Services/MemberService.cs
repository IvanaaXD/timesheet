using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.Common.DTOs;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Application.Exceptions;

namespace TimeSheet.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public MemberService(IMemberRepository memberRepository, IMapper mapper, IEmailService emailService, IPasswordHasher passwordHasher)
        {
            _memberRepository = memberRepository;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%";
            Random res = new Random();
            char[] password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[res.Next(validChars.Length)];
            }
            return new string(password);
        }

        public async Task<MemberDTO> GetMemberByIdAsync(Guid id)
        {
            var member = await _memberRepository.FindMemberByIdAsync(id);
            if (member == null) throw new NotFoundException($"Member with ID {id} not found.");

            return _mapper.Map<MemberDTO>(member);
        }

        public async Task<MemberDTO> GetMemberByUsernameAsync(string username)
        {
            var member = await _memberRepository.FindMemberByUsernameAsync(username);
            if (member == null) throw new NotFoundException($"Member with username {username} not found.");

            return _mapper.Map<MemberDTO>(member);
        }

        public async Task<MemberDTO> GetMemberByEmailAsync(string email)
        {
            var member = await _memberRepository.FindMemberByEmailAsync(email);
            if (member == null) throw new NotFoundException($"Member with email {email} not found.");

            return _mapper.Map<MemberDTO>(member);
        }

        public async Task<IEnumerable<MemberDTO>> GetAllMembersAsync()
        {
            var members = await _memberRepository.FindAllMembersAsync();
            return _mapper.Map<IEnumerable<MemberDTO>>(members);
        }

        public async Task<PagedList<MemberDTO>> GetAllMembersPagedAsync(PagedListDTO pagedListDTO)
        {
            var pagedMembers = await _memberRepository.FindAllMembersPagedAsync(
                pagedListDTO.PageNumber, pagedListDTO.PageSize, pagedListDTO.Order);

            var dtos = _mapper.Map<PagedList<MemberDTO>>(pagedMembers);

            return dtos;
        }

        public async Task<MemberDTO> CreateMemberAsync(MemberRequestDTO memberRequestDTO)
        {
            var member = _mapper.Map<Member>(memberRequestDTO);
            member.Id = Guid.NewGuid();

            string plainPassword = GenerateRandomPassword(10);
            member.Password = _passwordHasher.HashPassword(plainPassword);
            await _emailService.SendWelcomeEmailAsync(member.Email, plainPassword);

            await _memberRepository.AddMemberAsync(member);

            var createdMember = await _memberRepository.FindMemberByIdAsync(member.Id);
            return _mapper.Map<MemberDTO>(createdMember);
        }

        public async Task<MemberDTO> UpdateMemberAsync(Guid id, MemberRequestDTO memberRequestDTO)
        {
            var existingMember = await _memberRepository.FindMemberByIdAsync(id);
            if (existingMember == null) throw new NotFoundException($"Member with ID {id} not found.");

            _mapper.Map(memberRequestDTO, existingMember);
            await _memberRepository.UpdateMemberAsync(existingMember);

            var updatedMember = await _memberRepository.FindMemberByIdAsync(id);
            return _mapper.Map<MemberDTO>(updatedMember);
        }

        public async Task<MemberDTO> UpdateMemberPasswordAsync(Guid id, MemberRequestDTO memberRequestDTO)
        {
            var existingMember = await _memberRepository.FindMemberByIdAsync(id);
            if (existingMember == null) throw new NotFoundException($"Member with ID {id} not found.");

            string plainPassword = GenerateRandomPassword(10);
            existingMember.Password = _passwordHasher.HashPassword(plainPassword);
            await _emailService.SendPasswordUpdatedEmailAsync(existingMember.Email, plainPassword);

            _mapper.Map(memberRequestDTO, existingMember);
            await _memberRepository.UpdateMemberAsync(existingMember);

            var updatedMember = await _memberRepository.FindMemberByIdAsync(id);
            return _mapper.Map<MemberDTO>(updatedMember);
        }

        public async Task DeleteMemberAsync(Guid id)
        {
            var existingMember = await _memberRepository.FindMemberByIdAsync(id);
            if (existingMember == null) throw new NotFoundException($"Member with ID {id} not found.");

            await _memberRepository.DeleteMemberAsync(existingMember);
        }
    }
}