$(function () {

    $(".next-step").on("click", function () {

        var currentTab = $(".tab-pane.active");
        var currentForm = currentTab.find("form");
        var formId = currentForm.attr("id");

        if (formId == "frmShiftgroup") {
            var result = $('#' + formId).valid();
            if (!result) {
                return;
            }
        }
        else if (formId == "frmShifts") {
            var totalshift = $("#tblShifts tbody tr").length;
            if (totalshift == 0) {
                toastr.error("Please Add some shift(s)");
                return;
            }
            else {
                CollectInformationfromAllTabs();
            }
        }
        let nextTab = $('.nav-pills .nav-link.active').parent().next('li').find('button');
        nextTab.trigger('click');
    });

    $(".prev-step").on("click", function () {
        let prevTab = $('.nav-pills .nav-link.active').parent().prev('li').find('button');
        prevTab.trigger('click');
    });

    $("#frmShiftgroup").validate({
        highlight: function (element) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid").addClass("is-valid");
        },
        rules: {
            GroupCode: {
                required: true,
                minlength: 2,
                maxlength: 20
            },
            GroupName: {
                required: true,
                minlength: 2,
                maxlength: 100
            },
            RotationType: {
                required: true,
            },
            Description: {
                minlength: 2,
                maxlength: 200
            },
            EffectiveFrom: {
                required: true,
                date: true
            }
        },
        messages: {
            GroupCode: {
                required: "Group code required",
                minlength: "Description length must be 2 to 20 characters",
                maxlength: "Description length must be 2 to 20 characters"
            },
            GroupName: {
                required: "Group name required",
                minlength: "Description length must be 2 to 100 characters",
                maxlength: "Description length must be 2 to 100 characters"
            },
            RotationType: {
                required: "Select Shift Rotation Type",
            },
            Description: {
                minlength: "Description length must be 2 to 200 characters",
                maxlength: "Description length must be 2 to 200 characters"
            },
            EffectiveFrom: {
                required: "Effective from date required"
            }
        },
        highlight: function (element) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid").addClass("is-valid");
        },
        errorElement: "div",
        errorClass: "invalid-feedback",
        errorPlacement: function (error, element) {
            // place error message directly after the input inside form-group
            error.insertAfter(element);
        }
    });

    $("#frmShifts").validate({
        highlight: function (element) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid").addClass("is-valid");
        },
        rules: {
            ShiftGroupId: {
                required: true,
            },
            ShiftCode: {
                required: true,
                minlength: 2
            },
            ShiftName: {
                required: true,
            },
            StartTime: {
                required: true
            },
            EndTime: {
                required: true
            },
            WorkingHours: {
                required: true
            },
            IsNightShift: {
                required: true
            }
        },
        messages: {
            ShiftGroupId: {
                required: "Please select Shift Group",
            },
            ShiftCode: {
                required: "Shift Code is required",
                minlength: "Shift Code must be at least 2 characters"
            },
            ShiftName: {
                required: "Shift Name is required",
            },
            StartTime: {
                required: "Start Time is required"
            },
            EndTime: {
                required: "End Time is required"
            },
            WorkingHours: {
                required: "Working Hours is required"
            },
            IsNightShift: {
                required: "Night Shift is required"
            }
        },
        highlight: function (element) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid").addClass("is-valid");
        },
        errorElement: "div",
        errorClass: "invalid-feedback",
        errorPlacement: function (error, element) {
            // place error message directly after the input inside form-group
            error.insertAfter(element);
        }
    });
})
function calculateWorkingHours() {
    var start = $("#StartTime").val();
    var end = $("#EndTime").val();

    if (start != '' && end != '') {
        var startTime = new Date("1970-01-01T" + start + ":00");
        var endTime = new Date("1970-01-01T" + end + ":00");

        if (endTime < startTime) {
            endTime.setDate(endTime.getDate() + 1);
        }

        var diffMs = endTime - startTime;
        var totalMinutes = Math.floor(diffMs / 60000); // total minutes
        var totalHours = (totalMinutes / 60).toFixed(2); // decimal hours (2 decimals)

        $("#WorkingHours").val(totalHours); // e.g. 9.50
    }
}
$("#StartTime, #EndTime").on("change", calculateWorkingHours);

function pad2(number) {
    return (number < 10 ? '0' : '') + number;
}

$("#btnAdd").on("click", function () {

    var result = $("#frmShifts").valid();
    if (result) {
        var shiftId = $("#Id").val();
        var ShiftGroupId = $("#ShiftGroupId").val();
        var ShiftCode = $("#ShiftCode").val();
        var ShiftName = $("#ShiftName").val();
        var StartTime = $("#StartTime").val();
        var EndTime = $("#EndTime").val();
        var WorkingHours = $("#WorkingHours").val();
        var IsNightShift = $("#IsNightShift").val();
        var nightShift = $("#IsNightShift option:selected").text();
        var GracePeriodMins = $("#GracePeriodMins").val();

        var $row = `<tr>
        <td style="display:none">${shiftId == "" ? 0 : shiftId}</td>
        <td style="display:none">${ShiftGroupId == "" ? 0 : ShiftGroupId}</td>
        <td>${ShiftCode}</td>
        <td>${ShiftName}</td>
        <td>${StartTime}</td>
        <td>${EndTime}</td>
        <td>${WorkingHours}</td>
        <td style="display:none">${IsNightShift}</td>
        <td>${nightShift}</td>
        <td>${GracePeriodMins}</td>
        <td><button class="btn btn-sm btn-outline-secondary" id="btn-edit"><i class="fa-solid fa-pen-to-square"></i></button>
        <button class="btn btn-sm btn-outline-danger" id="btn-delete"><i class="fa-solid fa-trash"></i></button></td></tr>`;
        $("#tblShifts tbody").append($row);

        $("#ShiftGroupId").val('');
        $("#ShiftCode").val('');
        $("#ShiftName").val('');
        $("#StartTime").val('');
        $("#EndTime").val('');
        $("#WorkingHours").val('');
        $("#IsNightShift").val('');
        $("#GracePeriodMins").val('');
        $("#btnAdd").val("Add Shift");
    }
})

$(document).on("click", "#btn-edit", function () {
    var $row = $(this).closest("tr");
    var Shiftid = $row.find("td:eq(0)").text();
    var ShiftGroupid = $row.find("td:eq(1)").text();
    var ShiftCode = $row.find("td:eq(3)").text();
    var ShiftName = $row.find("td:eq(4)").text();
    var StartTime = $row.find("td:eq(5)").text();
    var EndTime = $row.find("td:eq(6)").text();
    var WorkingHours = $row.find("td:eq(7)").text();
    var IsNightShift = $row.find("td:eq(8)").text();
    var GracePeriod = $row.find("td:eq(10)").text();

    // Populate form fields again for editing
    $("#Id").val(Shiftid);
    $("#ShiftGroupId").val(ShiftGroupid);
    $("#ShiftCode").val(ShiftCode);
    $("#ShiftName").val(ShiftName);
    $("#StartTime").val(StartTime);
    $("#EndTime").val(EndTime);
    $("#WorkingHours").val(WorkingHours);
    $("#IsNightShift").val(IsNightShift);
    $("#GracePeriodMins").val(GracePeriod);
    $("#btnAdd").val("Update Shift");
    $row.remove();
});

$(document).on("click", "btn-delete", function () {
    $(this).closest("tr").remove();
    var totalshift = $("#tblShifts tbody tr").length;

    if (totalshift >= 0) {
        $("#btnSaveAll").attr("disabled", false);
    }
    else {
        $("#btnSaveAll").attr("disabled", true);
    }
});


function CollectInformationfromAllTabs() {

    $("#finalShiftGroupId").val($("#ShiftGroupId").val());
    $("#lblgroupCode").text($("#GroupCode").val());
    $("#lblgroupName").text($("#GroupName").val());
    $("#lblRotationType").text($("#RotationType option:selected").text());
    $("#RotateId").val($("#RotationType").val());
    $("#lblEffectiveFrom").text($("#EffectiveFrom").val());
    $("#lblEffectiveTo").text($("#EffectiveTo").val());
    $("#lblDescription").text($("#Description").val());

    $tblshiftTable = $("#tblfinalShift tbody");
    $("#tblShifts tbody tr").each(function (i) {
        var row = $(this).find("td");
        var shiftId = $(row[0]).text() ? "0" : $(row[0]).text();
        var $newrow = `<tr data-rowIndex="${i}">
        <td style="display:none">${shiftId}</td>
        <td>${$(row[2]).text()}</td>
        <td>${$(row[3]).text()}</td>
        <td>${$(row[4]).text()}</td>
        <td>${$(row[5]).text()}</td>
        <td>${$(row[6]).text()}</td>
        <td style="display:none">${$(row[7]).text()}</td>
        <td>${$(row[8]).text()}</td>
        <td>${$(row[9]).text()}</td></tr>`;
        $tblshiftTable.append($newrow);
    });

    if ($.fn.DataTable.isDataTable('#tblfinalShift')) {
        $('#tblfinalShift').DataTable().destroy();
    }
}

$("#btnSaveAll").on("click", function () {
    var url = $("#tblfinalShift").data("insert-url");
    var formData = new FormData();

    formData.append("SequenceNo", $("#SequenceNo").val());
    formData.append("RotationDays", $("#RotationDays").val());

    formData.append("ShiftGroup.Id", $("#finalShiftGroupId").val() || "0");
    formData.append("ShiftGroup.GroupCode", $("#lblgroupCode").text());
    formData.append("ShiftGroup.GroupName", $("#lblgroupName").text());
    formData.append("ShiftGroup.RotationType", $("#RotateId").val());
    formData.append("ShiftGroup.Description", $("#lblDescription").text());
    formData.append("ShiftGroup.EffectiveFrom", $("#lblEffectiveFrom").text());
    formData.append("ShiftGroup.EffectiveTo", $("#lblEffectiveTo").text());

    $("#tblfinalShift tbody tr").each(function (i) {
        var row = $(this).find("td");
        formData.append(`Shifts[${i}].Id`, $(row[0]).text());
        formData.append(`Shifts[${i}].ShiftCode`, $(row[1]).text());
        formData.append(`Shifts[${i}].ShiftName`, $(row[2]).text());
        formData.append(`Shifts[${i}].StartTime`, $(row[3]).text());
        formData.append(`Shifts[${i}].EndTime`, $(row[4]).text());
        formData.append(`Shifts[${i}].WorkingHours`, $(row[5]).text());
        formData.append(`Shifts[${i}].IsNightShift`, $(row[6]).text().toLowerCase() === "true");
        formData.append(`Shifts[${i}].GracePeriodMins`, $(row[8]).text());
    });


    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.result) {
                var homepage = $("#tblfinalShift").data("home-url");
                toastr.success(res.message);
                window.location.href = homepage;
            }
            else {
                toastr.error(res.message);
            }
        },
        error: function (err) {
            toastr.error(err);
        }
    });
})



