// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".utc-date-time").forEach(function (element) {
        const utcDateStr = element.getAttribute("data-utc");
        const localDate = new Date(utcDateStr + " UTC");
        const options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
        element.textContent = localDate.toLocaleString(undefined, options);
    });
});
