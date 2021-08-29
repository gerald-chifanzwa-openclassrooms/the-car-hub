// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('input.date-picker').pickadate({
    max: true,
    min: -365,
    formatSubmit: 'yyyy-mm-dd',
    hiddenName: true
});