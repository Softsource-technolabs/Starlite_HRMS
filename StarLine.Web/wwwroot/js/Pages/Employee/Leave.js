var $leaveTable;

document.addEventListener('DOMContentLoaded', function () {
    $leaveTable = $("#tblMyLeaves").dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": true,
        drawCallback: function () {
            $('.toggle-status').bootstrapToggle();
        },
        "ajax": {
            "url": $('#tblMyLeaves').data('get'),
            "type": "POST",
            "data": function (d) {
                // Send pagination, search, and sorting parameters to the server
                return {
                    draw: d.draw,
                    PageNumber: d.start / d.length + 1,  // Calculate page number
                    PageSize: d.length,           // Rows per page
                    SearchTerm: d.search.value,       // Search term (if any)
                    SortColumn: d.order[0] ? d.columns[d.order[0].column].data : "",  // Sorting column name
                    SortOrder: d.order[0] ? d.order[0].dir : ""  // Sorting direction
                };
            }
        },
        "columns": [
            { "data": "leaveTypeName", "name": "leaveTypeName", "autoWidth": true, orderable: true, },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return formatUTC(row.fromDate)
                }
            },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return formatUTC(row.toDate)
                }
            },
            { "data": "totalDays", "name": "totalDays", "autoWidth": true, orderable: true, },
            { "data": "durationName", "name": "durationName", "autoWidth": true, orderable: true, },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    let statusClass = '';
                    // Assign class based on statusName
                    switch (data.statusName.toLowerCase()) {
                        case 'approved':
                            statusClass = 'bg-success'; // green
                            break;
                        case 'rejected':
                            statusClass = 'bg-danger'; // red
                            break;
                        case 'pending':
                            statusClass = 'bg-warning'; // yellow
                            break;
                        default:
                            statusClass = 'bg-secondary'; // default gray
                    }
                    return `<span class="status-lable ${statusClass}">${data.statusName}</span>`;
                }
            },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return '<a class="btn btn-sm btn-outline-secondary" onclick="AddUpdateLeave(' + data.id + ')"><i class="fa-solid fa-pen-to-square"></i></a> '
                        + '<a class="btn btn-sm btn-outline-danger" id="btn-Delete" data-id=' + data.id + '><i class="fa-solid fa-trash"></i></a>';
                }
            }
        ]
    });

    $("#tblteamLeaves").dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": true,
        drawCallback: function () {
            $('.toggle-status').bootstrapToggle();
        },
        "ajax": {
            "url": $('#tblteamLeaves').data('get'),
            "type": "POST",
            "data": function (d) {
                // Send pagination, search, and sorting parameters to the server
                return {
                    draw: d.draw,
                    PageNumber: d.start / d.length + 1,  // Calculate page number
                    PageSize: d.length,           // Rows per page
                    SearchTerm: d.search.value,       // Search term (if any)
                    SortColumn: d.order[0] ? d.columns[d.order[0].column].data : "",  // Sorting column name
                    SortOrder: d.order[0] ? d.order[0].dir : ""  // Sorting direction
                };
            }
        },
        "columns": [
            { "data": "employeeName", "name": "employeeName", "autoWidth": true, orderable: true, },
            { "data": "leaveTypeName", "name": "leaveTypeName", "autoWidth": true, orderable: true, },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return formatUTC(row.fromDate)
                }
            },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return formatUTC(row.toDate)
                }
            },
            { "data": "totalDays", "name": "totalDays", "autoWidth": true, orderable: true, },
            { "data": "durationName", "name": "durationName", "autoWidth": true, orderable: true, },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    let statusClass = '';
                    // Assign class based on statusName
                    switch (data.statusName.toLowerCase()) {
                        case 'approved':
                            statusClass = 'bg-success'; // green
                            break;
                        case 'rejected':
                            statusClass = 'bg-danger'; // red
                            break;
                        case 'pending':
                            statusClass = 'bg-warning'; // yellow
                            break;
                        default:
                            statusClass = 'bg-secondary'; // default gray
                    }
                    return `<span class="status-lable ${statusClass}">${data.statusName}</span>`;
                }
            },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return '<a class="btn btn-sm btn-outline-secondary" onclick="LeaveApproval(' + data.id + ')"><i class="fa-solid fa-pen-to-square"></i></a> ';
                }
            }
        ]
    });

    $('.toggle-status').bootstrapToggle();
});

function AddUpdateLeave(id) {
    var url = $('#tblMyLeaves').data('create') + "?id=" + id;
    $.get(url, function (response) {
        if (response != null) {
            $("#starliteoffcanvas .offcanvas-body").html(response);
            var myOffcanvas = new bootstrap.Offcanvas(document.getElementById('starliteoffcanvas'));
            myOffcanvas.show();
            $("#starliteoffcanvas").css("width", "600px");
            $("#divHolidayTypeOther").hide();
            AddFormValidation();
        }
    })
}

function AddFormValidation() {
    $('#frmLeave').validate({
        rules: {
            LeaveTypeId: {
                required: true,
                min: 1
            },
            LeaveDuration: {
                required: true,
                min: 1
            },
            FromDate: {
                required: true,
                date: true
            },
            ToDate: {
                required: true,
                date: true,
            },
            TotalDays: {
                required: true,
                number: true,
                step: 0.01
            },
            ShortDescription: {
                required: true,
                minlength: 2,
                maxlength: 50
            }
        },
        messages: {
            LeaveTypeId: {
                required: "Select Leave Type",
                min: "Select Leave Type"
            },
            LeaveDuration: {
                required: "Select Leave Duration",
                min: "Select Leave Duration"
            },
            FromDate: {
                required: "Select Leave From Date"
            },
            ToDate: {
                required: "Select Leave To Date",
            },
            TotalDays: {
                required: "Enter Total Days",
                pattern: /^\d+(\.\d{1,2})?$/
            },
            ShortDescription: {
                required: "Enter short description",
                minlength: "Description should be at least 2 characters",
                maxlength: "Description should be max 50 characters"
            }
        },
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "div",
        errorPlacement: function (error, element) {
            error.addClass("invalid-feedback");
            error.insertAfter(element);
        }
    });
}

$("body").on("submit", "form", function (e) {
    e.preventDefault();
    var isValid = $("#frmLeave").valid(); // assumes you're using jQuery Validation plugin
    if (!isValid) {
        return;
    }
    $("#frmLeave")[0].submit();
});

function calculateTotalDays() {
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();

    if (fromDate && toDate) {
        var from = new Date(fromDate);
        var to = new Date(toDate);

        var diffTime = to - from;
        if (diffTime < 0) {
            $("#TotalDays").val(0); // negative date selection
        } else {
            var diffDays = diffTime / (1000 * 60 * 60 * 24) + 1; // +1 for inclusive count
            $("#TotalDays").val(diffDays);

            var leaveSelect = $("#LeaveDuration");
            leaveSelect.prop("disabled", false);

            if (diffDays > 1) {
                // only allow full-day leave
                leaveSelect.prop("disabled", true);
                leaveSelect.val("1"); // auto-select full
            } else if (diffDays === 1) {
                // allow firstHalf or secondHalf or full
                leaveSelect.prop("disabled", false);
            }
        }
    } else {
        $("#TotalDays").val(""); // reset if dates not selected
    }
}

// Bind calculation on change of either date field
$("body").on("blur", "#FromDate, #ToDate", calculateTotalDays);

$("body").on("change", "#LeaveDuration", function () {
    var leaveDuration = parseInt($(this).val());
    var leaveDurationText = $(this).find("option:selected").text(); // Get selected text
    var totalDaysInput = $("#TotalDays");
    var totalDays = parseFloat(totalDaysInput.val()); // Get current total days

    // Only adjust if totalDays is exactly 1
    if (totalDays <= 1) {
        if (leaveDuration > 1) {
            totalDaysInput.val(parseFloat(0.5)); // Half day = 0.5
        } else if (leaveDuration = 1) {
            totalDaysInput.val(parseFloat(1)); // Full day = 1
        }
    }
    // Optional: show error if user selects half-day but totalDays > 1
    else if (leaveDuration > 1 && totalDays > 1) {
        toastr.error(`Cannot apply ${leaveDurationText} leave as total days is more than 1`);
        // Reset the dropdown back to FullDay
        $(this).val(parseFloat(1));
    }
})


function LeaveApproval(id) {
    var url = $('#tblteamLeaves').data('create') + "?id=" + id;
    $.get(url, function (response) {
        if (response != null) {
            $("#starliteoffcanvas .offcanvas-body").html(response);
            var myOffcanvas = new bootstrap.Offcanvas(document.getElementById('starliteoffcanvas'));
            myOffcanvas.show();
            $("#starliteoffcanvas").css("width", "600px");
            $("#divHolidayTypeOther").hide();
        }
    })
}