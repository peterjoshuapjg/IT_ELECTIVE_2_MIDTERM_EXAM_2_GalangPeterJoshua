using ClinicPatientVisitMonitoringSystem.Models;
using ClinicPatientVisitMonitoringSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicPatientVisitMonitoringSystem.Controllers;

[Authorize]
public class PatientVisitController : Controller
{
    private readonly PatientVisitRepository _repository;

    public PatientVisitController(PatientVisitRepository repository) => _repository = repository;

    [HttpGet]
    public IActionResult Index(string? search)
    {
        ViewBag.Search = search;
        return View(_repository.Search(search));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PatientVisit { ArrivalDateTime = DateTime.Now });
    }

    [HttpGet]
    public IActionResult Patients(string? search)
    {
        ViewBag.Search = search;

        var patients = _repository.GetAll()
            .GroupBy(v => new { v.FirstName, v.LastName, v.ContactNumber })
            .Select(g => new PatientSummary
            {
                FirstName = g.Key.FirstName,
                LastName = g.Key.LastName,
                ContactNumber = g.Key.ContactNumber,
                Age = g.First().Age,
                Sex = g.First().Sex,
                Address = g.First().Address,
                TotalVisits = g.Count(),
                LastVisit = g.Max(v => v.ArrivalDateTime),
                LastPhysician = g.OrderByDescending(v => v.ArrivalDateTime).First().Physician,
                LastStatus = g.OrderByDescending(v => v.ArrivalDateTime).First().Status
            })
            .Where(p => string.IsNullOrWhiteSpace(search)
                || p.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || p.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || p.ContactNumber.Contains(search, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(p => p.LastVisit)
            .ToList();

        return View(patients);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PatientVisit model)
    {
        if (!ModelState.IsValid) return View(model);

        _repository.Add(model);
        TempData["Success"] = $"Patient visit {model.VisitNumber} was registered.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var visit = _repository.GetById(id);
        return visit is null ? NotFound() : View(visit);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(PatientVisit model)
    {
        if (!ModelState.IsValid) return View(model);

        if (!_repository.Update(model)) return NotFound();
        TempData["Success"] = $"Patient visit {model.VisitNumber} was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var visit = _repository.GetById(id);
        return visit is null ? NotFound() : View(visit);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Start(int id)
    {
        if (!_repository.StartConsultation(id))
            TempData["Error"] = "The consultation could not be started.";
        else
            TempData["Success"] = "Consultation marked as In Consultation.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Complete(int id)
    {
        var visit = _repository.GetById(id);
        return visit is null ? NotFound() : View(visit);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Complete(int id, string? notes)
    {
        var visit = _repository.GetById(id);
        if (visit is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(notes))
            visit.Notes = notes.Trim();

        if (!_repository.CompleteConsultation(id))
            TempData["Error"] = "The consultation could not be completed.";
        else
            TempData["Success"] = $"Consultation {visit.VisitNumber} was completed.";

        return RedirectToAction(nameof(Index));
    }
}