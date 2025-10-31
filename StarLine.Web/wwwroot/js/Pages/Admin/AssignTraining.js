
$(function () {
    $("#tblTrainingAssignmentSession").dataTable();
})

$('#sessionId').on("change", function () {
    var sessionId = $(this).val();
    if (sessionId) {
        $('#employeeContainer').html('<div class="text-info">Loading employees...</div>');
        $.get(`/Admin/TrainingAssignment/GetEligibleEmployees?sessionId=${sessionId}`, function (data) {
            $('#employeeContainer').html(data);
        });
    } else {
        $('#employeeContainer').html('<div class="text-muted">Select a session to load eligible employees...</div>');
    }
});

$(document).on("click", "#btnViewEmployee", function () {
    var sessionId = $(this).data("sessionid");
    $.get(`/Admin/TrainingAssignment/GetSessionEmployeeList?sessionId=${sessionId}`, function (data) {
        $('#hrmsModalBody').html(data);
        $('#hrmsSmallModel .modal-dialog').addClass('modal-lg');
        $('#hrmsSmallModel').modal('show');
    });
})

$("#btnSubmit").on("click", function () {
    var SessionId = $("#sessionId").val();
    var EmployeeIds = $('input[name="employeeIds"]:checked').length;

    if (SessionId == "") {
        toastr.error("Please select training session from list");
        return false;
    }
    if (EmployeeIds <= 0) {
        toastr.error("Please select employee from List");
        return false;
    }
})