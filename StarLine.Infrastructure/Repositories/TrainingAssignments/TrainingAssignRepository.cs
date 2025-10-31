using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StarLine.Core.Common;
using StarLine.Core.CommonService;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Repositories.TrainingAssignments
{
    public class TrainingAssignRepository(StarLiteContext context, FileStorageService fileStorage, IMapper mapper, IUserSession userSession, IOptions<AppSettings> settings) : ITrainingAssignRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly FileStorageService _fileStorage = fileStorage;
        private readonly IUserSession _userSession = userSession;
        private readonly AppSettings _settings = settings.Value;
        public async Task<BaseApiResponse> AssignEmployeesAsync(long sessionId, List<long> employeeIds)
        {
            foreach (var empId in employeeIds)
            {
                bool alreadyExists = await _context.TrainingAssignments
                    .AnyAsync(a => a.TrainingSessionId == sessionId && a.EmployeeId == empId);

                if (!alreadyExists)
                {
                    _context.TrainingAssignments.Add(new TrainingAssignment
                    {
                        TrainingSessionId = sessionId,
                        EmployeeId = empId,
                        AssignedDate = DateTime.UtcNow,
                        Status = (int)TrainingAssignStatus.Assigned,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = _userSession.Current.UserId
                    });
                }
            }

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new BaseApiResponse
                {
                    Success = true,
                    Message = "Employees successfully assigned to the training session."
                };
            }
            else
            {
                return new BaseApiResponse
                {
                    Success = false,
                    Message = "Failed to assign employees to the training session."
                };
            }
        }

        public async Task<List<TrainingAssignmentListModel>> GetAllAssignmentsAsync()
        {
            return await _context.TrainingAssignments.Include(a => a.TrainingSession).ThenInclude(s => s.Training)
                .Include(a => a.TrainingSession).ThenInclude(s => s.Trainer).GroupBy(a => new
                {
                    a.TrainingSession.Id,
                    a.TrainingSession.Training.Name,
                    a.TrainingSession.TrainingCode,
                    a.TrainingSession.Trainer.FirstName,
                    a.TrainingSession.Trainer.LastName
                })
                .Select(g => new TrainingAssignmentListModel
                {
                    Id = g.Key.Id,
                    TrainingName = g.Key.Name,
                    SessionCode = g.Key.TrainingCode,
                    TrainerName = g.Key.FirstName + " " + g.Key.LastName,
                    AssignedDate = g.Max(x => x.AssignedDate),
                    CompletionStatus =
                        g.All(x => x.Status == (int)TrainingAssignStatus.Completed) ? TrainingAssignStatus.Completed
                            : g.Any(x => x.Status == (int)TrainingAssignStatus.InProgress) ? TrainingAssignStatus.InProgress
                                : TrainingAssignStatus.Assigned
                })
                .OrderByDescending(x => x.AssignedDate)
                .ToListAsync();
        }

        public async Task<List<TrainingAssignmentListModel>> GetSessionAssignedEmployeeList(long sessionId)
        {
            return await _context.TrainingAssignments
            .Where(a => a.TrainingSessionId == sessionId)
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .Select(a => new TrainingAssignmentListModel
            {
                Id = a.Id,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                Department = a.Employee.Department.DepartmentName,
                CompletionStatus = (a.Status == (int)TrainingAssignStatus.Completed) ? TrainingAssignStatus.Completed
                            : (a.Status == (int)TrainingAssignStatus.InProgress) ? TrainingAssignStatus.InProgress
                                : TrainingAssignStatus.Assigned
            })
            .ToListAsync();
        }

        public async Task<List<TrainingSessionModel>> GetAllSessionsAsync()
        {
            var sessionList = new List<TrainingSessionModel>();
            var sessions = await _context.TrainingSessions.Include(_ => _.Training).Where(_ => !_.IsDeleted && _.IsActive).ToListAsync();
            foreach (var item in sessions)
            {
                if (item.EndDate != null)
                {
                    if (Convert.ToDateTime(item.EndDate) > DateTime.UtcNow)
                    {
                        sessionList.Add(_mapper.Map<TrainingSessionModel>(item));
                    }
                }
                sessionList.Add(_mapper.Map<TrainingSessionModel>(item));
            }
            return sessionList;
        }

        public Task<List<TrainingAssignmentModel>> GetAssignmentsBySessionAsync(long sessionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EmployeeModel>> GetEligibleEmployeesAsync(long trainingSessionId)
        {
            var session = await _context.TrainingSessions
        .Include(s => s.Training)
        .FirstOrDefaultAsync(s => s.Id == trainingSessionId);

            if (session == null)
                return new List<EmployeeModel>();

            var trainingId = session.TrainingId;

            // Step 2: Get employees with valid (non-expired) certificates for this training
            var employeesWithValidCerts = await _context.TrainingCertificates
                .Where(c => c.Id == trainingId && c.ValidUntil >= DateTime.UtcNow)
                .Select(c => c.EmployeeId)
                .Distinct()
                .ToListAsync();

            // Step 3: Return employees who are active and NOT in that list
            var eligibleEmployees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Where(e => e.IsActive && !employeesWithValidCerts.Contains(e.Id))
                .OrderBy(e => e.FirstName)
                .ToListAsync();


            return _mapper.Map<List<EmployeeModel>>(eligibleEmployees);
        }

        public async Task<List<TrainerSessionListModel>> GetAllSessionsByTrainerAsync(long trainerId)
        {
            var sessionList = new List<TrainerSessionListModel>();

            var sessions = await _context.TrainingSessions.Include(s => s.TrainingAssignments).Include(s => s.Training)
            .Where(s => s.TrainerId == trainerId).OrderByDescending(s => s.StartDate).ToListAsync();

            foreach (var item in sessions)
            {
                int status = item.TrainingAssignments.FirstOrDefault().Status;
                var model = new TrainerSessionListModel
                {
                    Id = item.Id,
                    TrainingName = item.Training.Name,
                    TrainingCode = item.TrainingCode,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    TotalParticipants = _context.TrainingAssignments.Count(a => a.TrainingSessionId == item.Id),
                    Status = status == (int)TrainingAssignStatus.Assigned ? TrainingAssignStatus.Assigned
                             : status == (int)TrainingAssignStatus.InProgress ? TrainingAssignStatus.InProgress
                             : TrainingAssignStatus.Completed
                };
                sessionList.Add(model);
            }
            return sessionList;
        }

        public async Task<bool> UpdateCompletionStatusAsync(List<TrainingSessionCompletionModel> Session)
        {
            foreach (var item in Session)
            {
                var assignment = await _context.TrainingAssignments.FindAsync(item.Id);
                if (assignment != null)
                {
                    assignment.Status = item.Status;
                    assignment.Remarks = item.Remarks;
                    assignment.CompletionDate = DateTime.UtcNow;
                    assignment.ModifiedBy = _userSession.Current.UserId;
                }
                await _context.SaveChangesAsync();
                await AddUpdateCertificateAsync(assignment.TrainingSessionId, assignment.EmployeeId);
            }
            return true;
        }

        private async Task<bool> AddUpdateCertificateAsync(long sessionId, long EmployeeId)
        {
            var completedAssignments = await _context.TrainingAssignments
                .Where(a => a.TrainingSessionId == sessionId && a.Status == (int)TrainingAssignStatus.Completed)
                .ToListAsync();

            foreach (var a in completedAssignments)
            {
                var CertificateExists = await _context.TrainingCertificates.FirstOrDefaultAsync(c => c.Id == a.Id);

                if (CertificateExists != null)
                {
                    CertificateExists.IsActive = false;
                    CertificateExists.Remarks = "Certificate Expired";
                    CertificateExists.UpdatedDate = DateTime.UtcNow;
                    _context.Update(CertificateExists);
                    await _context.SaveChangesAsync();
                }
                var cert = new TrainingCertificate
                {
                    EmployeeId = EmployeeId,
                    TrainingAssignedId = a.Id,
                    IssuedDate = DateTime.UtcNow,
                    ValidUntil = DateTime.UtcNow.AddYears(1),
                    IssuedBy = _userSession.Current.UserId,
                    Remarks = $"Training Held on {DateTime.Now.ToString("MM/dd/yyyy")} Certificate issued upon training completion",
                };
                _context.TrainingCertificates.Add(cert);
                await _context.SaveChangesAsync();
                await GenerateCertificate(cert.Id);
            }
            return true;
        }

        public async Task GenerateCertificate(long certificateId)
        {
            var certificate = await _context.TrainingCertificates.Include(_ => _.TrainingAssigned).ThenInclude(_ => _.Employee)
                .ThenInclude(_ => _.Department).Include(_ => _.TrainingAssigned.TrainingSession).ThenInclude(_ => _.Training)
                .FirstOrDefaultAsync(_ => _.Id == certificateId);

            if (certificate == null)
                throw new Exception("Certificate not found.");

            var employee = certificate.TrainingAssigned.Employee;
            var session = certificate.TrainingAssigned.TrainingSession;
            var training = session.Training;
            var trainer = await _context.Employees.FirstOrDefaultAsync(_ => _.Id == session.TrainerId);

            var pdf = Document.Create(c =>
            {
                c.Page(p =>
                {
                    p.Size(PageSizes.A4.Landscape());
                    p.Margin(2, Unit.Centimetre);
                    p.DefaultTextStyle(_ => _.FontSize(16));

                    p.Header().AlignCenter().Text("Starlite Pharma").FontFamily("Georgia").FontSize(24).Bold().Italic();

                    p.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(25);

                        col.Item().AlignCenter().Text("TRAINING COMPLETION CERTIFICATE")
                            .FontSize(24).Bold().FontColor(Colors.Blue.Medium);

                        col.Item().AlignCenter().Text($"This is to certify that")
                            .FontSize(16);

                        col.Item().AlignCenter().Text($"{employee.FirstName} {employee.LastName}")
                            .FontSize(28).Bold().FontColor(Colors.Black);

                        col.Item().AlignCenter().Text($"has successfully completed the training")
                            .FontSize(16);

                        col.Item().AlignCenter().Text($"{training.Name}")
                            .FontSize(20).Bold().FontColor(Colors.Green.Medium);

                        col.Item().AlignCenter().Text($"conducted by {trainer.FirstName} {trainer.LastName}")
                            .FontSize(16);

                        col.Item().AlignCenter().Text($"on {certificate.IssuedDate:dd MMM yyyy}")
                            .FontSize(14);

                        col.Item().AlignCenter().Text($"Certificate No: {certificate.CertificateNo}")
                            .FontSize(12).FontColor(Colors.Grey.Medium);
                    });

                    p.Footer().Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Authorized Signature");
                        row.RelativeItem().AlignRight().Text("HR Department");
                    });
                });
            });

            var certificateBytes = pdf.GeneratePdf();
            var fileName = $"Certificate_{certificate.CertificateNo}.pdf";


            var folderLocation = Path.Combine(_settings.Storage.CertificateLocation, fileName);

            var result = await _fileStorage.StoreFile(certificateBytes, folderLocation);
            if (result.Success)
            {
                certificate.CertificatePath = result.Data;
                certificate.IssuedDate = DateTime.UtcNow;
                _context.TrainingCertificates.Update(certificate);
                await _context.SaveChangesAsync();
            }
        }
    }
}
