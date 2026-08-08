using System.Security.Claims;
using ClinicPatientVisitMonitoringSystem.Models;
using ClinicPatientVisitMonitoringSystem.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicPatientVisitMonitoringSystem.Controllers;

public class AccountController : Controller
{
    private readonly UserRepository _users;

    public AccountController(UserRepository users) => _users = users;

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var user = _users.GetByUsername(model.Username);
        if (user is null || user.Password != model.Password)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "Reception")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties { IsPersistent = model.RememberMe };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "PatientVisit");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register() => View();

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (_users.UsernameExists(model.Username))
            ModelState.AddModelError(nameof(model.Username), "Username is already in use.");

        if (!ModelState.IsValid) return View(model);

        _users.Add(new User
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = model.Email.Trim(),
            Username = model.Username.Trim(),
            Password = model.Password
        });

        TempData["Success"] = "Registration successful. You can now log in.";
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpGet]
    public IActionResult Settings()
    {
        var user = _users.GetById(CurrentUserId);
        if (user is null) return RedirectToAction(nameof(Login));

        var model = new SettingsViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(SettingsViewModel model)
    {
        var user = _users.GetById(CurrentUserId);
        if (user is null) return RedirectToAction(nameof(Login));

        if (!ModelState.IsValid)
        {
            model.Username = user.Username;
            return View(nameof(Settings), model);
        }

        user.FirstName = model.FirstName.Trim();
        user.LastName = model.LastName.Trim();
        user.Email = model.Email.Trim();
        _users.Update(user);

        // Refresh the display name shown in the sidebar/topbar without requiring a fresh login.
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "Reception")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        TempData["Success"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Settings));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        var user = _users.GetById(CurrentUserId);
        if (user is null) return RedirectToAction(nameof(Login));

        if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
        {
            TempData["Error"] = "New password must be at least 6 characters long.";
            return RedirectToAction(nameof(Settings));
        }

        if (model.NewPassword != model.ConfirmNewPassword)
        {
            TempData["Error"] = "New password and confirmation do not match.";
            return RedirectToAction(nameof(Settings));
        }

        if (user.Password != model.CurrentPassword)
        {
            TempData["Error"] = "Current password is incorrect.";
            return RedirectToAction(nameof(Settings));
        }

        _users.UpdatePassword(user.Id, model.NewPassword);
        TempData["Success"] = "Your password has been changed.";
        return RedirectToAction(nameof(Settings));
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}