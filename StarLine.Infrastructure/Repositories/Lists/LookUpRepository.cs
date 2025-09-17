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
                return model.Select(_ => new SelectListItem
                {
                    Text = _.DepartmentName,
                    Value = _.Id.ToString()
                }).ToList();
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
                return model.Select(_ => new SelectListItem
                {
                    Text = _.Name,
                    Value = _.Id
                }).ToList();
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetAllUsers(string searchText)
        {
            var query = _context.Employees.Where(_ => _.IsActive == true && _.IsDeleted == false).AsQueryable();
            if(!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(_ => _.FirstName.Contains(searchText) || _.LastName.Contains(searchText));
            }
            var model = await query.ToListAsync();
            if (model != null)
            {
                return model.Select(_ => new SelectListItem
                {
                    Text = _.FirstName + " " + _.LastName,
                    Value = _.Id.ToString()
                }).ToList();
            }
            return null;
        }
    }
}
