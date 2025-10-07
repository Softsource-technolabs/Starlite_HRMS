// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatUTC(input) {
    const d = new Date(input);
    const mm = String(d.getUTCMonth() + 1).padStart(2, '0');
    const dd = String(d.getUTCDate()).padStart(2, '0');
    const yyyy = d.getUTCFullYear();
    return `${mm}/${dd}/${yyyy}`;
}

const HRMSLoader = {
    show: function () {
        $("#hrmsLoaderOverlay").addClass("active");
    },
    hide: function () {
        $("#hrmsLoaderOverlay").removeClass("active");
    }
};

$(document).on("submit", "form", function () {
    HRMSLoader.show();
});

// Global AJAX events (works for $.ajax, $.get, $.post, $.ajaxSetup, etc.)
$(document).ajaxStart(function () {
    HRMSLoader.show();
}).ajaxStop(function () {
    HRMSLoader.hide();
}).ajaxError(function () {
    HRMSLoader.hide(); // hide even if AJAX fails
});

function formatDate(dateInput) {
    var date = new Date(dateInput);

    var month = (date.getMonth() + 1).toString().padStart(2, '0'); // months are 0-based
    var day = date.getDate().toString().padStart(2, '0');
    var year = date.getFullYear();

    return month + '-' + day + '-' + year;
}

function formatDateTime(dateInput) {
    var date = new Date(dateInput);

    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var day = date.getDate().toString().padStart(2, '0');
    var year = date.getFullYear();

    var hours = date.getHours();
    var minutes = date.getMinutes().toString().padStart(2, '0');

    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours.toString().padStart(2, '0');

    return month + '-' + day + '-' + year + ' ' + hours + ':' + minutes + ' ' + ampm;
}

function formatTimeString(timeStr) {
    if (!timeStr) return "-";

    // Split by ":" (take only first 3 parts -> HH, MM, SS)
    let parts = timeStr.split(":");
    if (parts.length < 2) return timeStr; // fallback

    let hours = parseInt(parts[0], 10);
    let minutes = parseInt(parts[1], 10);
    let ampm = hours >= 12 ? "PM" : "AM";

    hours = hours % 12;
    hours = hours ? hours : 12; // 0 = 12 AM
    minutes = minutes < 10 ? "0" + minutes : minutes;

    return `${hours}:${minutes} ${ampm}`;
}
