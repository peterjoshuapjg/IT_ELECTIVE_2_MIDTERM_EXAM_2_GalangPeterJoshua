using Microsoft.AspNetCore.Mvc;

namespace ClinicPatientVisitMonitoringSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "PatientVisit");
}
