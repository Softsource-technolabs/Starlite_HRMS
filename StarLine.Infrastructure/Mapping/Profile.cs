using AutoMapper;
using StarLine.Core.Models;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Mapping
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<Department, DepartmentModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.DepartmentName, opt => opt.MapFrom(_ => _.DepartmentName))
                .ForMember(_ => _.Description, opt => opt.MapFrom(_ => _.Description))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap();

            CreateMap<Designation, DesignationModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.DepartmentId, opt => opt.MapFrom(_ => _.DepartmentId))
                .ForMember(_ => _.DesignationName, opt => opt.MapFrom(_ => _.DesignationName))
                .ForMember(_ => _.Description, opt => opt.MapFrom(_ => _.Description))
                .ForMember(_ => _.DepartmentName, opt => opt.MapFrom(_ => _.Department.DepartmentName))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap()
                .ForMember(dest => dest.Department, opt => opt.Ignore());

            CreateMap<Employee, EmployeeModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.EmployeeCode, opt => opt.MapFrom(_ => _.EmployeeCode))
                .ForMember(_ => _.UserImages, opt => opt.MapFrom(_ => _.UserImages))
                .ForMember(_ => _.FirstName, opt => opt.MapFrom(_ => _.FirstName))
                .ForMember(_ => _.LastName, opt => opt.MapFrom(_ => _.LastName))
                .ForMember(_ => _.Gender, opt => opt.MapFrom(_ => _.Gender))
                .ForMember(_ => _.DateOfBirth, opt => opt.MapFrom(_ => _.DateOfBirth))
                .ForMember(_ => _.Email, opt => opt.MapFrom(_ => _.Email))
                .ForMember(_ => _.PhoneNumber, opt => opt.MapFrom(_ => _.PhoneNumber))
                .ForMember(_ => _.DepartmentId, opt => opt.MapFrom(_ => _.DepartmentId))
                .ForMember(_ => _.DesignationId, opt => opt.MapFrom(_ => _.DesignationId))
                .ForMember(_ => _.ReportingManagerId, opt => opt.MapFrom(_ => _.ReportingManagerId))
                .ForMember(_ => _.JoiningDate, opt => opt.MapFrom(_ => _.JoiningDate))
                .ForMember(_ => _.EmploymentType, opt => opt.MapFrom(_ => _.EmploymentType))
                .ForMember(_ => _.Qualification, opt => opt.MapFrom(_ => _.Qualification))
                .ForMember(_ => _.ExperienceInYears, opt => opt.MapFrom(_ => _.ExperienceInYears))
                .ForMember(_ => _.LicenseNumber, opt => opt.MapFrom(_ => _.LicenseNumber))
                .ForMember(_ => _.BloodGroup, opt => opt.MapFrom(_ => _.BloodGroup))
                .ForMember(_ => _.EmergencyContactName, opt => opt.MapFrom(_ => _.EmergencyContactName))
                .ForMember(_ => _.EmergencyContactNumber, opt => opt.MapFrom(_ => _.EmergencyContactNumber))
                .ForMember(_ => _.CurrentAddress, opt => opt.MapFrom(_ => _.CurrentAddress))
                .ForMember(_ => _.PermanentAddress, opt => opt.MapFrom(_ => _.PermanentAddress))
                .ForMember(_ => _.RoleId, opt => opt.MapFrom(_ => _.AspNetUser.Roles.FirstOrDefault().Id))
                .ForMember(_ => _.RoleName, opt => opt.MapFrom(_ => _.AspNetUser.Roles.FirstOrDefault().Name))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap()
                .ForMember(dest => dest.AspNetUser, opt => opt.Ignore());

            CreateMap<Team, TeamModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.Name, opt => opt.MapFrom(_ => _.Name))
                .ForMember(_ => _.DepartmentId, opt => opt.MapFrom(_ => _.DepartmentId))
                .ForMember(_ => _.Description, opt => opt.MapFrom(_ => _.Description))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.Department.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.Department.IsDeleted))
                .ReverseMap()
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.TeamMembers, opt => opt.Ignore());

            CreateMap<Holiday, HolidayModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.Name, opt => opt.MapFrom(_ => _.Name))
                .ForMember(_ => _.HolidayDate, opt => opt.MapFrom(_ => _.HolidayDate))
                .ForMember(_ => _.HolidayType, opt => opt.MapFrom(_ => _.HolidayType))
                .ForMember(_ => _.HolidayTypename, opt => opt.MapFrom(_ => _.HolidayTypename))
                .ForMember(_ => _.Ismandatory, opt => opt.MapFrom(_ => _.Ismandatory))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap();

            CreateMap<LeaveType, LeaveTypeModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.TypeName, opt => opt.MapFrom(_ => _.TypeName))
                .ForMember(_ => _.Description, opt => opt.MapFrom(_ => _.Description))
                .ForMember(_ => _.MaxDaysPerMonth, opt => opt.MapFrom(_ => _.MaxDaysPerMonth))
                .ForMember(_ => _.IsCarryForward, opt => opt.MapFrom(_ => _.IsCarryForward))
                .ForMember(_ => _.GenderContraint, opt => opt.MapFrom(_ => _.GenderContraint))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap();
        }
    }
}
