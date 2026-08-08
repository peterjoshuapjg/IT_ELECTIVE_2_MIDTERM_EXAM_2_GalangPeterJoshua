// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Toggles a password field between hidden (dots) and visible (plain text).
// Expects the toggle button to be a sibling of the <input> inside a ".password-field" wrapper.
function togglePasswordVisibility(button) {
    var wrapper = button.closest('.password-field');
    if (!wrapper) return;

    var input = wrapper.querySelector('input');
    var icon = button.querySelector('i');
    if (!input) return;

    var isHidden = input.type === 'password';
    input.type = isHidden ? 'text' : 'password';

    if (icon) {
        icon.classList.toggle('bi-eye', !isHidden);
        icon.classList.toggle('bi-eye-slash', isHidden);
    }
    button.setAttribute('aria-label', isHidden ? 'Hide password' : 'Show password');
}

// Switches the app between light and dark themes and remembers the choice per browser.
function toggleTheme(isDark) {
    document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');
    localStorage.setItem('clinicflow-theme', isDark ? 'dark' : 'light');
}

// Keep the Settings page toggle in sync with the currently applied theme.
document.addEventListener('DOMContentLoaded', function () {
    var themeSwitch = document.getElementById('themeSwitch');
    if (themeSwitch) {
        themeSwitch.checked = document.documentElement.getAttribute('data-theme') === 'dark';
    }
});