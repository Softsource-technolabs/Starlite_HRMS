const attendaceEndPoint = "/Admin/Home/GetAttendace";
const markclockIn = "/Admin/Home/MarkclockIn";
const markclockOut = "/Admin/Home/MarkclockOut";

$(async function () {
    await GetLeaves();
    await getAttendance();
    await GetAllAttendance();
})

async function GetLeaves() {
    $("#tblHolidays").dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": true,
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
        let table = $('#tblAttendance').DataTable();
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

async function getAttendance() {
    $.get(attendaceEndPoint, function (response) {
        if (response.success == true) {
            var shift = response.shift;
            $("#spnOfficeTime").text(shift.shiftName);
            if (shift.isClockedIn && !shift.isclockedOut) {
                $("#spnStatus").removeClass("text-warning");
                $("#spnStatus").text(shift.status).addClass("text-success");
                $("#btnclockIn").prop("disabled", true);
                $("#btnclockOut").prop("disabled", false);
            }
            if (shift.isclockedOut) {
                $("#spnStatus").removeClass("text-success");
                $("#spnStatus").text(shift.status).addClass("text-warning");
                $("#btnclockOut").prop("disabled", true);
                $("#btnclockIn").prop("disabled", false);
            }
        }
    })
}

$("#btnclockIn").on("click", async function () {
    $.get(markclockIn, async function (response) {
        if (response == true) {
            $("#btnclockIn").prop("disabled", true);
            $("#btnclockOut").prop("disabled", false);
            toastr.success("Attendance Mark as clock in");
            await getAttendance();
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
            await getAttendance();
            await GetAllAttendance();
        }
        else {
            toastr.error("Please try again after sometime");
        }
    })
})