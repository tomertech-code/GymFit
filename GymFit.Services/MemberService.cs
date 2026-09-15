using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymFit.Application.DTOs;
using GymFit.Application.Interfaces;
using GymFit.Application.ViewModels;
using GymFit.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymFit.Services
{

    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
        {
            var members = await _unitOfWork.Members.GetActiveMembersAsync();
            return _mapper.Map<IEnumerable<MemberDto>>(members);
        }

        public async Task<MemberDto?> GetMemberByIdAsync(int id)
        {
            var member = await _unitOfWork.Members.GetMemberWithDetailsAsync(id);
            return member != null ? _mapper.Map<MemberDto>(member) : null;
        }

        public async Task<MemberDto?> GetMemberByUserIdAsync(string userId)
        {
            var member = await _unitOfWork.Members.GetMemberByUserIdAsync(userId);
            return member != null ? _mapper.Map<MemberDto>(member) : null;
        }

        public async Task<bool> CreateMemberAsync(MemberViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !user.IsActive) return false;

                var existing = await _unitOfWork.Members.FindAsync(m => m.UserId == user.Id);
                if (existing.Any()) return false;

                var member = new Member
                {
                    UserId = user.Id,
                    Address = model.Address,
                    EmergencyContact = model.EmergencyContact,
                    MedicalConditions = model.MedicalConditions,
                    AssignedTrainerId = model.AssignedTrainerId,
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };

                var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!roleResult.Succeeded)
                    return false;

                try
                {
                    await _unitOfWork.Members.AddAsync(member);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                catch
                {
                    await _userManager.RemoveFromRoleAsync(user, "Member");
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateMemberAsync(MemberViewModel model)
        {
            try
            {
                var member = await _unitOfWork.Members.GetByIdAsync(model.Id);
                if (member == null) return false;

                member.Address = model.Address;
                member.EmergencyContact = model.EmergencyContact;
                member.MedicalConditions = model.MedicalConditions;
                member.AssignedTrainerId = model.AssignedTrainerId;
                member.PrimaryBranchId = model.PrimaryBranchId;

                var user = await _userManager.FindByIdAsync(member.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }

                _unitOfWork.Members.Update(member);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteMemberAsync(int id)
        {
            try
            {
                var member = await _unitOfWork.Members.GetByIdAsync(id);
                if (member == null) return false;

                member.IsActive = false;
                var user = await _userManager.FindByIdAsync(member.UserId);
                if (user != null)
                    user.IsActive = false;
                _unitOfWork.Members.Update(member);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetActiveMembersCountAsync()
        {
            return await _unitOfWork.Members.CountActiveAsync();
        }
    }
}
