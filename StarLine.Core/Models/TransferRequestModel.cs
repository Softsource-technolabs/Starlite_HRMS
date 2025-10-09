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
        public int Status { get; set; }
        public bool CurrentManagerApproval { get; set; }
        public bool ReceivingManagerApproval { get; set; }
        public bool Hrapproval { get; set; }
        public string Reason { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public string EmployeeName { get; set; }
        public string FromDepartmentName { get; set; }
        public string ToDepartmentName { get; set; }
    }
}
