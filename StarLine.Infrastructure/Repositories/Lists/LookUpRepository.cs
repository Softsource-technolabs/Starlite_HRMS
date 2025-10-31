using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Repositories.Lists
{
    public class LookUpRepository(StarLiteContext context) : ILookUpRepository
    {
        private readonly StarLiteContext _context = context;
        public async Task<List<SelectListItem>> GetAllDepartment(string searchText)
        {
            var query = _context.Departments.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.DepartmentName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.DepartmentName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Department", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetAllDesignation(string searchText)
        {
            var query = _context.Designations.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.DesignationName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.DesignationName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Designation", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetAllRoles(string searchText)
        {
            var query = _context.AspNetRoles.AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.Name.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.Name,
                    Value = _.Id
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Role", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetAllTrainers(string searchText)
        {
            var query = _context.Employees.Include(_ => _.Designation).Where(_ => _.IsActive == true && _.IsDeleted == false && _.Designation.IsTrainer == true).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.FirstName.Contains(searchText) || _.LastName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.FirstName + " " + _.LastName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Trainer", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetAllTraining(string searchText)
        {
            var query = _context.Trainings.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.Name.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.Name,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Training", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetAllUsers(string searchText)
        {
            var query = _context.Employees.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.FirstName.Contains(searchText) || _.LastName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.FirstName + " " + _.LastName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select User", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetReportingManagers(string searchText)
        {
            var query = _context.Employees.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.FirstName.Contains(searchText) || _.LastName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.FirstName + " " + _.LastName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Reporting Manager", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetShift(string searchText, long shiftgroupId)
        {
            var query = _context.Shifts.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.ShiftName.Contains(searchText));
            }
            if (shiftgroupId > 0)
            {
                query = query.Where(_ => _.ShiftGroupId == shiftgroupId);
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.ShiftName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Shift", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetShiftGroups(string searchText)
        {
            var query = _context.ShiftGroups.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.GroupName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.GroupName,
                    Value = _.Id.ToString()
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Shift Group", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetTeambyDepartmentId(long departmentId)
        {
            var query = _context.Teams.AsQueryable();
            if (departmentId > 0)
            {
                query = query.Where(_ => _.DepartmentId == departmentId);
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                var result = model.Select(_ => new SelectListItem
                {
                    Text = _.Name,
                    Value = _.Id.ToString(),
                }).ToList();

                result.Insert(0, new SelectListItem { Text = "Select Team", Value = "0" });
                return result;
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetTeamMembers(long departmentId, long currentUserId)
        {
            var emp = await _context.Employees.FirstOrDefaultAsync(_ => _.Id == currentUserId);
            var employees = await _context.Employees.Include(_ => _.Department).Include(_ => _.TeamMembers).ThenInclude(_ => _.Team)
                .Where(_ => _.IsActive == true && _.IsDeleted == false && _.DepartmentId == departmentId).ToListAsync();

            if (employees.Count > 0)
            {
                employees = employees.Where(_ => _.Id != emp.ReportingManagerId).ToList();
                var result = employees.Select(_ => new SelectListItem
                {
                    Text = _.FirstName + " " + _.LastName,
                    Value = _.Id.ToString()
                }).ToList();
                result.Insert(0, new SelectListItem { Text = "Select Employee", Value = "0" });
                return result;
            }
            else
            {
                return new List<SelectListItem> { new SelectListItem { Text = "No Employees Found", Value = "0" } };
            }
        }
    }
}
