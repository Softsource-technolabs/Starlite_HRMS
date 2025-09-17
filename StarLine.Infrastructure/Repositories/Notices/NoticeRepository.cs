using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Core.StorageService;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Notices
{
    public class NoticeRepository(StarLiteContext context, IMapper mapper, IUserSession userSession, FileStorageService fileStorageService, IOptions<AppSettings> settings) : INoticeRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;
        private readonly FileStorageService _fileStorageService = fileStorageService;
        private readonly AppSettings _appSettings = settings.Value;
        private async Task<BaseApiResponse> AddNotice(NoticeModel notice)
        {
            var model = _mapper.Map<Notice>(notice);
            if (notice.File != null)
            {
                var fileResult = await _fileStorageService.StoreFile(notice.File, _appSettings.Storage.NoticeDocsLocation);
                if (!fileResult.Success)
                    return new BaseApiResponse { Success = false, Message = fileResult.Message };

                model.ActualFileName = notice.File.FileName;
                model.FileName = fileResult.Data;
            }
            model.NoticeType = (int)notice.NoticeType;
            model.AudienceType = (int)notice.AudienceType;
            await _context.Notices.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new BaseApiResponse { Message = "Notice added successfully", Success = true };
            }
            return new BaseApiResponse { Message = "Notice not added", Success = false };
        }

        public async Task<BaseApiResponse> AddUpdateNotice(NoticeModel notice)
        {
            if (notice.Id > 0)
                return await UpdateNotice(notice);
            else
                return await AddNotice(notice);
        }

        public async Task<BaseApiResponse> DeleteNotice(long id)
        {
            var notice = await _context.Notices.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (notice != null)
            {
                notice.IsDeleted = true;
                notice.DeletedBy = _userSession.Current.UserId;
                _context.Notices.Update(notice);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Message = "Notice deleted successfully", Success = true };
                else
                    return new BaseApiResponse { Success = false, Message = "Notice not deleted" };
            }
            return new BaseApiResponse { Success = false, Message = "Notice not found" };
        }

        public async Task<PagedResponse<List<NoticeModel>>> GetAllNotices(PaginationModel model)
        {
            var query = _context.Notices.Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.Title.Contains(model.StrSearch) || _.NoticeText.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Notice).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<NoticeModel>>(data);
            return new PagedResponse<List<NoticeModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<ApiPostResponse<NoticeModel>> GetNoticeById(long id)
        {
            var notice = await _context.Notices.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (notice != null)
            {
                var model = _mapper.Map<NoticeModel>(notice);
                model.FileName = _appSettings.Storage.NoticeDocsLocation + "/" + notice.FileName; //_fileStorageService.GetFilePath(notice.FileName, _appSettings.Storage.NoticeDocsLocation);
                return new ApiPostResponse<NoticeModel> { Data = model, Message = "Notice Found", Success = true };
            }
            return new ApiPostResponse<NoticeModel> { Success = false, Message = "Notice not found" };
        }

        public async Task<BaseApiResponse> ToggleStatusNotice(long id)
        {
            var notice = await _context.Notices.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (notice != null)
            {
                notice.IsActive = !notice.IsActive;
                notice.UpdatedBy = _userSession.Current.UserId;
                _context.Notices.Update(notice);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Message = "Notice status changed successfully", Success = true };
                else
                    return new BaseApiResponse { Success = false, Message = "Notice status not changed" };
            }
            return new BaseApiResponse { Success = false, Message = "Notice not found" };
        }

        private async Task<BaseApiResponse> UpdateNotice(NoticeModel notice)
        {
            var existingNotice = await _context.Notices.Where(_ => _.Id == notice.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (notice != null)
            {
                var model = _mapper.Map(notice, existingNotice);
                model.NoticeType = (int)notice.NoticeType;
                model.AudienceType = (int)notice.AudienceType;
                if (notice.File != null)
                {
                    var fileResult = await _fileStorageService.StoreFile(notice.File, _appSettings.Storage.NoticeDocsLocation);
                    if (!fileResult.Success)
                        return new BaseApiResponse { Success = false, Message = fileResult.Message };

                    model.ActualFileName = notice.File.FileName;
                    model.FileName = fileResult.Data;
                }
                
                _context.Notices.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Message = "Notice updated successfully", Success = true };
                else
                    return new ApiPostResponse<NoticeModel> { Message = "Notice not found", Success = false };
            }
            return new ApiPostResponse<NoticeModel> { Success = false, Message = "Notice not found" };
        }
    }
}
