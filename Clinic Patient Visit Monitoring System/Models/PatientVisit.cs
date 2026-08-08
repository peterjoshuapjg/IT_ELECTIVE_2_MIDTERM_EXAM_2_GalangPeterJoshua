using System.ComponentModel.DataAnnotations;

namespace ClinicPatientVisitMonitoringSystem.Models;

public class PatientVisit
{
    public int Id { get; set; }

    [Display(Name = "Visit Number")]
    public string VisitNumber { get; set; } = string.Empty;

    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, Range(0, 120)]
    public int Age { get; set; }

    [Required]
    public string Sex { get; set; } = string.Empty;

    [Required, Phone, Display(Name = "Contact Number")]
    public string ContactNumber { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Physician { get; set; } = string.Empty;

    [Required, Display(Name = "Visit Type")]
    public string VisitType { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime), Display(Name = "Arrival Date & Time")]
    public DateTime ArrivalDateTime { get; set; }

    [Display(Name = "Consultation End Time"), DataType(DataType.DateTime)]
    public DateTime? ConsultationCompletedDateTime { get; set; }

    [Required]
    public string Status { get; set; } = "Waiting";

    [Required, Display(Name = "Chief Complaint")]
    public string ChiefComplaint { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
