// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('input.date-picker').pickadate({
    selectMonths: true,
    selectYears: true,
    max: true,
    min: new Date(1, 1, 1990)
});