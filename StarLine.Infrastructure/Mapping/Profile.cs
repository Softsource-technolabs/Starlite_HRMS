using AutoMapper;
using StarLine.Core.Models;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Mapping
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<DepartmentModel, Department>()
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
                .ForMember(_ => _.HierarchyLevel, opt => opt.MapFrom(_ => _.HierarchyLevel))
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
                .ForMember(_ => _.shiftId, opt => opt.MapFrom(_ => _.ShiftId))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ForMember(_ => _.DepartmentName, opt => opt.MapFrom(_ => _.Department.DepartmentName))
                .ReverseMap()
                .ForMember(dest => dest.AspNetUser, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore());

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

            CreateMap<HolidayModel, Holiday>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.Name, opt => opt.MapFrom(_ => _.Name))
                .ForMember(_ => _.HolidayDate, opt => opt.MapFrom(_ => _.HolidayDate))
                .ForMember(_ => _.HolidayType, opt => opt.MapFrom(_ => _.HolidayType))
                .ForMember(_ => _.HolidayTypename, opt => opt.MapFrom(_ => _.HolidayTypename))
                .ForMember(_ => _.Ismandatory, opt => opt.MapFrom(_ => _.Ismandatory))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap();

            CreateMap<LeaveTypeModel, LeaveType>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.TypeName, opt => opt.MapFrom(_ => _.TypeName))
                .ForMember(_ => _.Description, opt => opt.MapFrom(_ => _.Description))
                .ForMember(_ => _.MaxDaysPerMonth, opt => opt.MapFrom(_ => _.MaxDaysPerMonth))
                .ForMember(_ => _.IsCarryForward, opt => opt.MapFrom(_ => _.IsCarryForward))
                .ForMember(_ => _.GenderContraint, opt => opt.MapFrom(_ => _.GenderContraint))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap();

            CreateMap<Leaf, LeaveModel>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.EmployeeId, opt => opt.MapFrom(_ => _.EmployeeId))
                .ForMember(_ => _.LeaveTypeId, opt => opt.MapFrom(_ => _.LeaveTypeId))
                .ForMember(_ => _.FromDate, opt => opt.MapFrom(_ => _.FromDate))
                .ForMember(_ => _.ToDate, opt => opt.MapFrom(_ => _.ToDate))
                .ForMember(_ => _.TotalDays, opt => opt.MapFrom(_ => _.TotalDays))
                .ForMember(_ => _.Status, opt => opt.MapFrom(_ => _.Status))
                .ForMember(_ => _.RejectReason, opt => opt.MapFrom(_ => _.RejectReason))
                .ForMember(_ => _.ShortDescription, opt => opt.MapFrom(_ => _.ShortDescription))
                .ForMember(_ => _.LeaveTypeName, opt => opt.MapFrom(_ => _.LeaveType.TypeName))
                .ForMember(_ => _.EmployeeName, opt => opt.MapFrom(_ => _.Employee.FirstName + " " + _.Employee.LastName))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(_ => _.IsActive))
                .ForMember(_ => _.IsDeleted, opt => opt.MapFrom(_ => _.IsDeleted))
                .ReverseMap()
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.LeaveType, opt => opt.Ignore());

            CreateMap<ShiftGroupModel, ShiftGroup>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.GroupCode, opt => opt.MapFrom(_ => _.GroupCode))
                .ForMember(_ => _.GroupName, opt => opt.MapFrom(_ => _.GroupName))
                .ForMember(_ => _.RotationType, opt => opt.MapFrom(_ => _.RotationType))
                .ForMember(_ => _.Description, opt => opt.MapFrom(_ => _.Description))
                .ForMember(_ => _.EffectiveFrom, opt => opt.MapFrom(_ => _.EffectiveFrom))
                .ForMember(_ => _.EffectiveTo, opt => opt.MapFrom(_ => _.EffectiveTo))
                .ReverseMap();

            CreateMap<ShiftModel, Shift>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.ShiftCode, opt => opt.MapFrom(_ => _.ShiftCode))
                .ForMember(_ => _.ShiftName, opt => opt.MapFrom(_ => _.ShiftName))
                .ForMember(_ => _.StartTime, opt => opt.MapFrom(_ => _.StartTime))
                .ForMember(_ => _.EndTime, opt => opt.MapFrom(_ => _.EndTime))
                .ForMember(_ => _.WorkingHours, opt => opt.MapFrom(_ => _.WorkingHours))
                .ForMember(_ => _.IsNightShift, opt => opt.MapFrom(_ => _.IsNightShift))
                .ForMember(_ => _.GracePeriodMins, opt => opt.MapFrom(_ => _.GracePeriodMins))
                .ReverseMap();

            CreateMap<ShiftGroupMappingModel, ShiftGroupMapping>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.ShiftGroupId, opt => opt.MapFrom(_ => _.ShiftGroupId))
                .ForMember(_ => _.ShiftId, opt => opt.MapFrom(_ => _.ShiftId))
                .ForMember(_ => _.SequenceNo, opt => opt.MapFrom(_ => _.SequenceNo))
                .ForMember(_ => _.RotationDays, opt => opt.MapFrom(_ => _.RotationDays))
                .ReverseMap();

            CreateMap<BranchModel, Branch>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.BranchCode, opt => opt.MapFrom(_ => _.BranchCode))
                .ForMember(_ => _.BranchName, opt => opt.MapFrom(_ => _.BranchName))
                .ForMember(_ => _.Address, opt => opt.MapFrom(_ => _.Address))
                .ForMember(_ => _.City, opt => opt.MapFrom(_ => _.City))
                .ForMember(_ => _.State, opt => opt.MapFrom(_ => _.State))
                .ForMember(_ => _.Country, opt => opt.MapFrom(_ => _.Country))
                .ForMember(_ => _.ContactNumber, opt => opt.MapFrom(_ => _.ContactNumber))
                .ForMember(_ => _.EmailAddress, opt => opt.MapFrom(_ => _.EmailAddress))
                .ForMember(_ => _.ParentBranchId, opt => opt.MapFrom(_ => _.ParentBranchId))
                .ReverseMap();

            CreateMap<NoticeModel, Notice>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.Title, opt => opt.MapFrom(_ => _.Title))
                .ForMember(_ => _.NoticeText, opt => opt.MapFrom(_ => _.NoticeText))
                .ForMember(_ => _.NoticeType, opt => opt.MapFrom(_ => (int)_.NoticeType))
                .ForMember(_ => _.DeliveryMode, opt => opt.MapFrom(_ => (int)_.DeliveryMode))
                .ForMember(_ => _.AcknoledgeRequired, opt => opt.MapFrom(_ => _.AcknoledgeRequired))
                .ForMember(_ => _.HasAttachment, opt => opt.MapFrom(_ => _.HasAttachment))
                .ForMember(_ => _.FileName, opt => opt.MapFrom(_ => _.FileName))
                .ForMember(_ => _.AudienceType, opt => opt.MapFrom(_ => (int)_.AudienceType))
                .ForMember(_ => _.AudienceTypeValue, opt => opt.MapFrom(_ => _.AudienceTypeValue))
                .ForMember(_ => _.PublishDate, opt => opt.MapFrom(_ => _.PublishDate))
                .ForMember(_ => _.ExpireDate, opt => opt.MapFrom(_ => _.ExpireDate))
                .ReverseMap();

            CreateMap<NoticeRecipientModel, NoticeRecipient>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.NoticeId, opt => opt.MapFrom(_ => _.NoticeId))
                .ForMember(_ => _.UserId, opt => opt.MapFrom(_ => _.UserId))
                .ForMember(_ => _.IsAcknoledged, opt => opt.MapFrom(_ => _.IsAcknoledged))
                .ForMember(_ => _.ReadAt, opt => opt.MapFrom(_ => _.ReadAt))
                .ForMember(_ => _.AcknowledgeDate, opt => opt.MapFrom(_ => _.AcknowledgeDate))
                .ReverseMap();

            CreateMap<AttendanceModel, Attendance>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.EmployeeId, opt => opt.MapFrom(_ => _.EmployeeId))
                .ForMember(_ => _.Attendacedate, opt => opt.MapFrom(_ => _.Attendacedate))
                .ForMember(_ => _.InTime, opt => opt.MapFrom(_ => _.InTime))
                .ForMember(_ => _.OutTime, opt => opt.MapFrom(_ => _.OutTime))
                .ForMember(_ => _.Status, opt => opt.MapFrom(_ => _.Status))
                .ForMember(_ => _.LateComing, opt => opt.MapFrom(_ => _.LateComing))
                .ForMember(_ => _.Remarks, opt => opt.MapFrom(_ => _.Remarks))
                .ReverseMap();

            CreateMap<TransferRequestModel, TransferRequest>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(_ => _.EmployeeId, opt => opt.MapFrom(_ => _.EmployeeId))
                .ForMember(_ => _.FromDepartmentId, opt => opt.MapFrom(_ => _.FromDepartmentId))
                .ForMember(_ => _.ToDepartmentId, opt => opt.MapFrom(_ => _.ToDepartmentId))
                .ForMember(_ => _.Reason, opt => opt.MapFrom(_ => _.Reason))
                .ForMember(_ => _.Status, opt => opt.MapFrom(_ => _.Status))
                .ForMember(_ => _.CurrentManagerApproval, opt => opt.MapFrom(_ => _.CurrentManagerApproval))
                .ForMember(_ => _.ReceivingManagerApproval, opt => opt.MapFrom(_ => _.ReceivingManagerApproval))
                .ForMember(_ => _.Hrapproval, opt => opt.MapFrom(_ => _.Hrapproval))
                .ForMember(_ => _.EffectiveDate, opt => opt.MapFrom(_ => _.EffectiveDate))
                .ReverseMap();
        }
    }
}
