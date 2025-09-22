using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Branches
{
    public class BranchRepository(StarLiteContext context, IMapper mapper, IUserSession userSession) : IBranchRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;

        public async Task<long> AddUpdateBranch(BranchModel branch)
        {
            long result = 0;
            if(branch.Id > 0)
            {
                result = await UpdateBranch(branch);
            }
            else
            {
                result = await AddBranch(branch);
            }
            return result;
        }

        public async Task<BaseApiResponse> DeleteBranch(long id)
        {
            var branch = await _context.Branches.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (branch != null)
            {
                branch.IsDeleted = !branch.IsDeleted;
                branch.DeletedBy = _userSession.Current.UserId;
                _context.Branches.Update(branch);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Branch deleted successfully" };
                }
                return new BaseApiResponse { Success = false, Message = "Branch not deleted" };
            }
            return new BaseApiResponse { Success = false, Message = "Branch not found" };
        }

        public async Task<PagedResponse<List<BranchModel>>> GetAllBranchs(PaginationModel model)
        {
            var query = _context.Branches.Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.BranchCode.Contains(model.StrSearch) || _.BranchName.Contains(model.StrSearch)
                || _.Address.Contains(model.StrSearch) || _.City.Contains(model.StrSearch) || _.State.Contains(model.StrSearch) || _.Country.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Branch).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<BranchModel>>(data);
            return new PagedResponse<List<BranchModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<BranchModel> GetBranchById(long id)
        {
            var branch = await _context.Branches.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (branch != null)
            {
                var model = _mapper.Map<BranchModel>(branch);
                return model;
            }
            return null;
        }

        public async Task<List<BranchModel>> GetBranchList()
        {
            var branch = await _context.Branches.Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if (branch != null)
            {
                var model = _mapper.Map<List<BranchModel>>(branch);
                return model;
            }
            return null;
        }

        public async Task<BaseApiResponse> ToggleStatusBranch(long id)
        {
            var branch = await _context.Branches.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (branch != null)
            {
                branch.IsActive = !branch.IsActive;
                branch.UpdatedBy = _userSession.Current.UserId;
                _context.Branches.Update(branch);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Branch Status successfully" };
                }
                return new BaseApiResponse { Success = false, Message = "Branch Status not deleted" };
            }
            return new BaseApiResponse { Success = false, Message = "Branch not found" };
        }

        private async Task<long> AddBranch(BranchModel branch)
        {
            var model = _mapper.Map<Branch>(branch);
            await _context.Branches.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            return model.Id;
        }

        private async Task<long> UpdateBranch(BranchModel branch)
        {
            var model = await _context.Branches.Where(_ => _.Id == branch.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (branch != null)
            {
                var objbranch = _mapper.Map(branch, model);
                objbranch.UpdatedBy = _userSession.Current.UserId;
                _context.Branches.Update(objbranch);
                return model.Id;
            }
            return 0;
        }
    }
}
