const attendaceEndPoint = "/Home/GetShiftDetails";
const markclockIn = "/Home/MarkclockIn";
const markclockOut = "/Home/MarkclockOut";
const viewAnnouncement = "/Home/ViewAnnounceMent"

$(async function () {
    await GetShiftDetails();
    await GetHolidays();
    await GetAllAttendance();
    await GetAllAnnouncement();
})

async function GetHolidays() {
    $("#tblHolidays").dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": true,
        pageLength: 5,
        drawCallback: function () {
            $('.toggle-status').bootstrapToggle();
        },
        "ajax": {
            "url": $('#tblHolidays').data('get'),
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
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return formatUTC(row.holidayDate)
                }
            },
            { "data": "name", "name": "name", "autoWidth": true, orderable: true, },
            { "data": "typeName", "name": "typeName", "autoWidth": true, orderable: true, }
        ]
    });
}

async function GetAllAttendance() {

    var url = $('#tblAttendance').data('get');

    $.get(url, function (response) {
        let table = $('#tblAttendance').DataTable({
            pageLength: 5
        });
        table.clear().draw();

        // Append rows
        $.each(response, function (index, row) {
            table.row.add([
                row.employeeName,
                row.attendacedate,
                formatTimeString(row.inTime),
                formatTimeString(row.outTime)
            ]).draw(false);
        });
    })
}

async function GetShiftDetails() {
    $.get(attendaceEndPoint, function (response) {
        if (response.success == true) {
            var data = response.data;
            if (data.isClockedIn && !data.isclockedOut) {
                $("#spnStatus").removeClass("text-warning");
                $("#spnStatus").text(data.status).addClass("text-success");
                $("#btnclockIn").prop("disabled", true);
                $("#btnclockOut").prop("disabled", false);
            }
            if (data.isclockedOut) {
                $("#spnStatus").removeClass("text-success");
                $("#spnStatus").text(data.status).addClass("text-warning");
                $("#btnclockOut").prop("disabled", true);
                $("#btnclockIn").prop("disabled", false);
            }

            var shift = response.data.shift;
            if (shift != null) {
                var container = $("#divShiftDetails");
                container.html("");
                const div = document.createElement("div");
                div.className = 'row mt-2';
                div.innerHTML = `<div class="col-md-4"><div class="row"><div class="col-md-12"><strong>Shift Group Name:</strong> ${shift.shiftGroup.groupName}</div><div class="col-md-12 mt-2"><strong>Shift Name:</strong> ${shift.shifts[0].shiftName}</div></div></div>
                                 <div class="col-md-4"><div class="row"><div class="col-md-12"><strong>Shift Time:</strong> ${formatTimeString(shift.shifts[0].startTime)} to ${formatTimeString(shift.shifts[0].endTime)}</div><div class="col-md-12 mt-2"><strong>Grace Period: </strong> ${shift.shifts[0].gracePeriodMins}</div></div></div>
                                 <div class="col-md-4"><div class="row"><div class="col-md-12"><strong>Shift Rotation Type:</strong> ${shift.shiftGroup.rotationTypeName}</div></div></div>`;
                container.append(div);
            }
        }
    })
}

async function GetAllAnnouncement() {

    var url = $('#tblAnnoncement').data('get');

    $.get(url, function (response) {
        if (response.length > 0) { $("#tblAnnoncement tbody").html(''); }
        var table = $('#tblAnnoncement').DataTable();
        table.page.len(5).draw();
        // Append rows
        $.each(response, function (index, row) {
            table.row.add([
                row.title,
                formatUTC(row.publishDate),
                `<a href=# class="btn btn-sm btn-outline-secondary w-100" onclick="OpenAnnouncement(${row.id})">View</a>`
            ]).draw(false);
        });
    })
}

$("#btnclockIn").on("click", async function () {
    $.get(markclockIn, async function (response) {
        if (response == true) {
            $("#btnclockIn").prop("disabled", true);
            $("#btnclockOut").prop("disabled", false);
            toastr.success("Attendance Mark as clock in");
            await GetShiftDetails();
            await GetAllAttendance();
        }
        else {
            toastr.error("Please try again after sometime");
        }
    })
})

$("#btnclockOut").on("click", async function () {
    $.get(markclockOut, async function (response) {
        if (response == true) {
            $("#btnclockOut").prop("disabled", true);
            $("#btnclockIn").prop("disabled", false);
            toastr.success("Attendance Mark as clock out");
            await GetShiftDetails();
            await GetAllAttendance();
        }
        else {
            toastr.error("Please try again after sometime");
        }
    })
})

function OpenAnnouncement(id) {
    var url = viewAnnouncement + "?Id=" + id;
    $.get(url, function (response) {
        if (response != false) {
            $("#hrmsModalBody").html(response);
            $("#hrmsSmallModel .modal-dialog").addClass("modal-xl");
            $("#hrmsSmallModel").modal("show");
        }
        else {
            toastr.error("error while opening announcement.");
        }
    })
}