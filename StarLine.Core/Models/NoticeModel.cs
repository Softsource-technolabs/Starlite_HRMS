using Microsoft.AspNetCore.Http;
using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StarLine.Core.Models
{
    public class NoticeModel : BaseEntity
    {
        [Display(Name ="Title")]
        [Required(ErrorMessage ="Notice Title required")]
        public string Title { get; set; }
        [Display(Name = "Notice")]
        [Required(ErrorMessage = "Notice Text required")]
        public string NoticeText { get; set; }
        [Display(Name = "Notice Type")]
        [Required(ErrorMessage = "Notice Type required")]
        public NoticeType NoticeType { get; set; }
        [Display(Name = "Delivery Mode")]
        [Required(ErrorMessage = "Delivery Mode required")]
        public DeliveryMode DeliveryMode { get; set; }
        [Display(Name = "Acknoledgement Required?")]
        [Required(ErrorMessage = "select value from dropdown")]
        public bool? AcknoledgeRequired { get; set; }
        [Display(Name = "Has Attachment?")]
        [Required(ErrorMessage = "select value from dropdown")]
        public bool? HasAttachment { get; set; }
        [Display(Name = "File Name")]
        [RequiredIf("HasAttachment","true",ErrorMessage ="File required when you select attachment to yes")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" })]
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        [Display(Name ="Audience Type Name")]
        public string AudienceTypeValue { get; set; }
        public string ActualFileName { get; set; }
        [Display(Name = "Audience Type")]
        [Required(ErrorMessage = "Audience Type required")]
        public AudienceType AudienceType { get; set; }
        public DateTime? PublishDate { get; set; }
        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }
        public string NoticeTypeName => CommonFunctions.GetDisplayName<NoticeType>((int)NoticeType);
        public string DeliveryModeName => CommonFunctions.GetDisplayName<DeliveryMode>((int)DeliveryMode);
        public string AudienceTypeName => CommonFunctions.GetDisplayName<AudienceType>((int)AudienceType);
    }
}
