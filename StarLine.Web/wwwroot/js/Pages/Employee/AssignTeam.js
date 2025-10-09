$(function () {
    $("#tblemployee").dataTable();
})

function assignTeam(empId) {
    var url = $("#tblemployee").data("url") + "?id=" + empId;
    $.get(url, function (response) {
        if (response != false) {
            $("#hrmsModalBody").html(response);
            $("#hrmsSmallModel").modal("show");
            addValidation();
            $("#DepartmentId").trigger("change");
        }
        else {
            toastr.error("error while opening announcement.");
        }
    });
}

$(document).on("change", "#shiftGroupId", function () {
    var getShiftURL = $("#hfgetShiftURL").val();
    getShiftURL = getShiftURL + "?id=" + $(this).val();
    $.get(getShiftURL, function (response) {
        const dropdown = $('#shiftId');
        dropdown.empty();

        response.forEach(item => {
            dropdown.append(`<option value="${item.value}">${item.text}</option>`);
        });
    });
});

$(document).on("change", "#DepartmentId", function () {
    var teamURL = $("#hfGetTeamsUrl").val();
    if (parseInt($(this).val()) > 0) {
        teamURL = teamURL + "?id=" + $(this).val();
        $.get(teamURL, function (response) {
            const dropdown = $('#TeamId');
            dropdown.empty();

            response.forEach(item => {
                dropdown.append(`<option value="${item.value}">${item.text}</option>`);
            });
        });
    }
});
function addValidation() {
    $("#frmTeamAssign").validate({
        rules: {
            DepartmentId: {
                required: true,
                min: 1
            },
            DesignationId: {
                required: true,
                min: 1
            },
            ReportingManagerId: {
                required: true,
                min: 1
            },
            TeamId: {
                required: true,
                min: 1
            },
            shiftGroupId: {
                required: true,
                min: 1
            },
            shiftId: {
                required: true,
                min: 1
            }
        },
        messages: {
            DepartmentId: {
                required: "Select Department",
                min: "Select Department"
            },
            DesignationId: {
                required: "Select Designation",
                min: "Select Designation"
            },
            ReportingManagerId: {
                required: "Select Reporting Manager",
                min: "Select Reporting Manager"
            },
            TeamId: {
                required: "Select Team",
                min: "Select Team"
            },
            shiftGroupId: {
                required: "Select Shift Group",
                min: "Select Shift Group"
            },
            shiftId: {
                required: "Select Shift",
                min: "Select Shift"
            }
        },
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "div",
        errorPlacement: function (error, element) {
            error.addClass("invalid-feedback");
            error.insertAfter(element);
        }
    })
}

$(document).on("click", "#btnTeamSubmit", function () {
    var isvalid = $("#frmTeamAssign").valid();
    if (isvalid) {
        var formData = new FormData();
        formData.append("EmployeeId", $("#Id").val());
        formData.append("DepartmentId", $("#DepartmentId").val());
        formData.append("DesignationId", $("#DesignationId").val());
        formData.append("ReportingManagerId", $("#ReportingManagerId").val());
        formData.append("TeamId", $("#TeamId").val());
        formData.append("ShiftGroupId", $("#shiftGroupId").val());
        formData.append("ShiftId", $("#shiftId").val());

        fetch('/Employee/EmployeeTeamAssign', {  // Change URL to your controller action
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            debugger;
            if (data.isSuccess) {
                $("#hrmsSmallModel").modal("hide");
                window.location.href = "/Employee/Index";
                toastr.success(data.message);
            }
            else
                toastr.error(data.message);
        })
        .catch(error => console.error('Error:', error));
    }
})