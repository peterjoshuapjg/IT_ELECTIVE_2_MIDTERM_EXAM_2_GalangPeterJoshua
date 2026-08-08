namespace ClinicPatientVisitMonitoringSystem.Models;

public class PatientSummary
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Sex { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TotalVisits { get; set; }
    public DateTime LastVisit { get; set; }
    public string LastPhysician { get; set; } = string.Empty;
    public string LastStatus { get; set; } = string.Empty;
}