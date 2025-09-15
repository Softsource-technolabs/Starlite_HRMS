using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Repositories.Shifts
{
    public class ShiftRepository(StarLiteContext context, IMapper mapper, IUserSession userSession) : IShiftRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;

        public async Task<ApiPostResponse<long>> AddorUpdateShift(ShiftWizardModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    ShiftGroup shiftGroup;

                    // 🔹 Check if group exists (update) or new (insert)
                    if (model.ShiftGroup.Id > 0)
                    {
                        shiftGroup = await _context.ShiftGroups.FirstOrDefaultAsync(s => s.Id == model.ShiftGroup.Id);

                        if (shiftGroup == null)
                        {
                            return new ApiPostResponse<long> { Success = false, Message = "Shift group not found" };
                        }
                        var groupModel = _mapper.Map<ShiftGroup>(model.ShiftGroup);
                        shiftGroup = _mapper.Map(shiftGroup, groupModel);
                        shiftGroup.UpdatedBy = _userSession.Current.UserId;
                        _context.ShiftGroups.Update(shiftGroup);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Insert new group
                        shiftGroup = _mapper.Map<ShiftGroup>(model.ShiftGroup);
                        shiftGroup.CreatedBy = _userSession.Current.UserId;
                        _context.ShiftGroups.Add(shiftGroup);
                        await _context.SaveChangesAsync(); // Save first so we get GroupId
                    }

                    // 🔹 Handle Shift mappings
                    foreach (var shiftModel in model.Shifts)
                    {

                        // Inserting Update Shift
                        var shift = new Shift();
                        var existingShift = await _context.Shifts.FirstOrDefaultAsync(_ => _.Id == shiftModel.Id);

                        if (existingShift != null)
                        {
                            var shiftMappedModel = _mapper.Map<Shift>(shiftModel);
                            shift = _mapper.Map(existingShift, shiftMappedModel);
                            shift.UpdatedBy = _userSession.Current.UserId;
                            _context.Shifts.Update(shift);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            // Insert new mapping
                            shift = _mapper.Map<Shift>(shiftModel);
                            shift.CreatedBy = _userSession.Current.UserId;
                            await _context.Shifts.AddAsync(shift);
                            await _context.SaveChangesAsync();
                        }


                        //Insert update shiftGroup and Shift Mapping

                        var existingMapping = await _context.ShiftGroupMappings.FirstOrDefaultAsync(_ => _.ShiftGroupId == shiftGroup.Id && _.ShiftId == shift.Id);

                        if (existingMapping != null)
                        {
                            existingMapping.SequenceNo = model.SequenceNo;
                            existingMapping.RotationDays = model.RotationDays;
                            existingMapping.UpdatedBy = _userSession.Current.UserId;
                            _context.ShiftGroupMappings.Update(existingMapping);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var mapping = new ShiftGroupMapping
                            {
                                ShiftGroupId = shiftGroup.Id,
                                ShiftId = shift.Id,
                                RotationDays = model.RotationDays,
                                SequenceNo = model.SequenceNo,
                                CreatedBy = _userSession.Current.UserId
                            };

                            await _context.ShiftGroupMappings.AddAsync(mapping);
                            await _context.SaveChangesAsync();
                        }
                    }

                    await transaction.CommitAsync();

                    return new ApiPostResponse<long> { Success = true, Message = "shift and shift group details are saved", Data = shiftGroup.Id };
                }
                catch (Exception ex)
                {

                    await transaction.RollbackAsync();
                    return new ApiPostResponse<long> { Success = false, Message = "Error while saving shift and shift group" };
                }
            }
        }

        public Task<PagedResponse<List<ShiftGroupModel>>> GetAllShiftGroups(PaginationModel model)
        {
            throw new NotImplementedException();
        }
    }
}
