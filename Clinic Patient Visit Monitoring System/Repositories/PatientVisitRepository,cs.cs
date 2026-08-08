using ClinicPatientVisitMonitoringSystem.Models;

namespace ClinicPatientVisitMonitoringSystem.Repositories;

public class PatientVisitRepository
{
    private static readonly List<PatientVisit> Visits = new();
    private static int _nextId = 1;

    public List<PatientVisit> GetAll() =>
        Visits.OrderByDescending(v => v.ArrivalDateTime).ToList();

    public PatientVisit? GetById(int id) => Visits.FirstOrDefault(v => v.Id == id);

    public List<PatientVisit> Search(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAll();

        query = query.Trim();
        return Visits.Where(v =>
                v.VisitNumber.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                v.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                v.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                v.Physician.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                v.Status.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                v.VisitType.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(v => v.ArrivalDateTime)
            .ToList();
    }

    public PatientVisit Add(PatientVisit visit)
    {
        visit.Id = _nextId++;
        visit.VisitNumber = $"V-{DateTime.Now:yyyyMMdd}-{visit.Id:0000}";
        visit.ArrivalDateTime = visit.ArrivalDateTime == default ? DateTime.Now : visit.ArrivalDateTime;
        visit.Status = "Waiting";
        Visits.Add(visit);
        return visit;
    }

    public bool Update(PatientVisit visit)
    {
        var existing = GetById(visit.Id);
        if (existing is null) return false;

        existing.FirstName = visit.FirstName;
        existing.LastName = visit.LastName;
        existing.Age = visit.Age;
        existing.Sex = visit.Sex;
        existing.ContactNumber = visit.ContactNumber;
        existing.Address = visit.Address;
        existing.Physician = visit.Physician;
        existing.VisitType = visit.VisitType;
        existing.ArrivalDateTime = visit.ArrivalDateTime;
        existing.ChiefComplaint = visit.ChiefComplaint;
        existing.Notes = visit.Notes;
        return true;
    }

    public bool StartConsultation(int id)
    {
        var visit = GetById(id);
        if (visit is null || visit.Status == "Completed") return false;
        visit.Status = "In Consultation";
        return true;
    }

    public bool CompleteConsultation(int id)
    {
        var visit = GetById(id);
        if (visit is null || visit.Status == "Completed") return false;
        visit.Status = "Completed";
        visit.ConsultationCompletedDateTime = DateTime.Now;
        return true;
    }
}
