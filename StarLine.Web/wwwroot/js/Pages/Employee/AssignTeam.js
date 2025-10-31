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

        var url = $("#tblemployee").data("assignteam")
        fetch(url, {  // Change URL to your controller action
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

function generateTransfer(employeeId) {
    var url = $("#tblemployee").data("transfer") + "?id=" + employeeId;
    $.get(url, function (response) {
        if (response.isSuccess != false) {
            $("#hrmsModalBody").html(response);
            $("#hrmsSmallModel").modal("show");
            $("#EmployeeId").val(employeeId).trigger("change");
            $("#EmployeeId").prop("disabled", "disabled");
            $("#FromDepartmentId").prop("disabled", "disabled");
            $("#Status").trigger("change");
            AddTransferFormValidation();
        }
        else {
            toastr.error("error while opening announcement.");
        }
    });
}

function AddTransferFormValidation() {
    $("#frmTransferForm").validate({
        rules: {
            EmployeeId: {
                required: true,
                min: 1
            },
            FromDepartmentId: {
                required: true,
                min: 1
            },
            ToDepartmentId: {
                required: true,
                min: 1
            },
            Status: {
                required: true,
                min: 1
            },
            EffectiveDate: {
                required: true,
            },
            Reason: {
                required: true,
                minlength: 2,
                maxlength: 100
            }
        },
        messages: {
            EmployeeId: {
                required: "Please select employee",
                min: "Please select employee"
            },
            FromDepartmentId: {
                required: "Please from department",
                min: "Please from department"
            },
            ToDepartmentId: {
                required: "Please to department",
                min: "Please to department",
            },
            Status: {
                required: "Please status",
                min: "Please status"
            },
            EffectiveDate: {
                required: "Please select effective date",
            },
            Reason: {
                required: "Reason required",
                minlength: "minimum 2 characters required",
                maxlength: "Maximum 100 characters allowed"
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

$(document).on("click", "#submitTransfer", function () {
    var isvalid = $("#frmTransferForm").valid();
    if (isvalid) {
        $("#EmployeeId").removeProp("disabled");
        $("#FromDepartmentId").removeProp("disabled");
        var formData = new FormData();
        formData.append("EmployeeId", $("#EmployeeId").val());
        formData.append("FromDepartmentId", $("#FromDepartmentId").val());
        formData.append("ToDepartmentId", $("#ToDepartmentId").val());
        formData.append("Status", 1);
        formData.append("CurrentManagerApproval", true);
        formData.append("ReceivingManagerApproval", false);
        formData.append("EffectiveDate", $("#EffectiveDate").val());
        formData.append("Hrapproval", false);
        formData.append("Reason", $("#Reason").val());

        var url = $("#tblemployee").data("submittransfer")
        fetch(url, { method: 'POST', body: formData })
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