using Microsoft.AspNetCore.Mvc;
using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class TrainingSessionModel : BaseEntity
    {
        [Display(Name = "Training")]
        [Required(ErrorMessage = "Please select training from list")]
        public long? TrainingId { get; set; }
        [Display(Name = "Training Name")]
        public string TrainingName { get; set; }
        [Display(Name = "Training Code")]
        public string TrainingCode { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please enter category")]
        public string Category { get; set; }
        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }
        [Display(Name = "Trainer")]
        [Required(ErrorMessage = "Please select trainer from list")]
        public long? TrainerId { get; set; }
        [Display(Name = "Trainer Name")]
        public string TrainerName { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Start date required")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [Remote("checkEndDate", "TrainingSession", AdditionalFields = "StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Duration (Hours)")]
        [Required(ErrorMessage = "Please enter duration in hours")]
        public decimal? DurationHours { get; set; }
        [Display(Name = "Mode")]
        [Required(ErrorMessage = "Please select mode from list")]
        public int? Mode { get; set; }
        [Display(Name = "Location")]
        [Required(ErrorMessage = "Please enter location")]
        public string Location { get; set; }
        [Display(Name = "Max Participants")]
        [Required(ErrorMessage = "Please enter maximum number of participants")]
        public int? MaxParticipants { get; set; }
    }

    public class TrainerSessionListModel
    {
        public long Id { get; set; }
        public string TrainingName { get; set; }
        public string TrainingCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalParticipants { get; set; }
        public TrainingAssignStatus Status { get; set; }
    }

    public class TrainingSessionCompletionModel
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
    }
}
