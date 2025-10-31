using StarLine.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarLine.Core.Models
{
    public class TransferRequestModel : BaseEntity
    {
        public long EmployeeId { get; set; }
        public long FromDepartmentId { get; set; }
        public long ToDepartmentId { get; set; }
        public TransferStatus CurrentManagerApproval { get; set; }
        public TransferStatus ReceivingManagerApproval { get; set; }
        public TransferStatus Hrapproval { get; set; }
        public string Reason { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public string EmployeeName { get; set; }
        public string FromDepartmentName { get; set; }
        public string ToDepartmentName { get; set; }
        public string HrManagerName => "Hr Manager"; // This can be fetched from user service based on HR approval
        public string FromDepartmentManager { get; set; }
        public string ToDepartmentManager { get; set; }
        public long fromDepartmentManagerId { get; set; }
        public long toDepartmentManagerId { get; set; }
    }
}
