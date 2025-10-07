$(function () {

    $('#NoticeText').summernote({
        height: 500,
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'italic', 'underline', 'clear', 'fontname']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['insert', ['link', 'table', 'hr']], // removed picture & video
            ['view', ['fullscreen', 'codeview']]
        ],
        callbacks: {
            onChange: function (contents, $editable) {
                $('#NoticeText').val(contents); // keeps textarea in sync
            }
        }
    });

    $.validator.addMethod("summernoteRequired", function (value, element) {
        return !$('#NoticeText').summernote('isEmpty');
    }, "Notice Text required");

    $('#frmNotice').validate({
        rules: {
            NoticeText: {
                summernoteRequired: true
            }
        }
    });

    toggleFileUpload();
    $("#HasAttachment").on("change", toggleFileUpload);

    SetAudienceTypeValue();
    $("#AudienceType").on("change", SetAudienceTypeValue);

    var audienceTypeValue = $("#AudienceTypeValue").val();
    if (audienceTypeValue != undefined && audienceTypeValue != "" && audienceTypeValue != 0) {
        var audienceType = $("#AudienceType option:selected").text();

        if (audienceType === "Department") {
            GetDepartmentName(audienceTypeValue);
        }
        else if (audienceType === "Role") {
            GetRoleName(audienceTypeValue);
        }
        else {
            GetUserName(audienceTypeValue);
        }
    }
});

function toggleFileUpload() {
    var hasAttachment = $("#HasAttachment").val();
    if (hasAttachment === 'true') {
        $("#divfileUpload").show();
    } else {
        $("#divfileUpload").hide();
    }
}

document.getElementById('File').addEventListener('change', function () {
    const allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif|\.bmp|\.pdf|\.doc|\.docx|\.xls|\.xlsx|\.ppt|\.pptx)$/i;
    const filePath = this.value;

    if (!allowedExtensions.exec(filePath)) {
        toastr.error('Invalid file type! Please upload an image, PDF, Word, Excel, or PowerPoint file.');
        this.value = ''; // clear file input
    }
});

function SetAudienceTypeValue() {
    var audienceType = $("#AudienceType option:selected").text();
    if (audienceType == "Select Audience Type") {
        return
    }
    if (audienceType === 'All') {
        $("#AudienceTypeValue").val("0");
        $("#divaudience").hide();
    } else {
        $("#txtaudienceValue").val("");
        $("#divaudience").show();
        var placeHolderText = `Enter ${audienceType} Name`;
        $("#txtaudienceValue").prop("placeholder", placeHolderText);
        SetAutoCompleteSource();
    }
}

function SetAutoCompleteSource() {
    var audienceType = $("#AudienceType option:selected").text();
    var url = "";

    if (audienceType === "Department") {
        url = "/api/departments";
    }
    else if (audienceType === "Role") {
        url = "/api/roles";
    }
    else {
        url = "/api/users";
    }

    $("#txtaudienceValue").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: url,
                type: "GET",
                data: { searchText: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.text,  // user sees this
                            value: item.text,  // textbox gets this
                            id: item.value
                        };
                    }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            // when user selects an item
            $("#AudienceTypeValue").val(ui.item.id);   // save ID to hidden field
            $("#txtaudienceValue").val(ui.item.label); // ensure name is shown
            return false; // prevents overriding value with "id"
        }
    });
}

function GetDepartmentName(departmentId) {
    $.get("/Admin/Notice/GetDepartment?id=" + departmentId, function (response) {
        if (response.result) {
            $("#txtaudienceValue").val(response.data.departmentName);
        }
    })
}

function GetRoleName(roleId) {
    $.get("/Admin/Notice/GetRole?id=" + roleId, function (response) {
        if (response.result) {
            $("#txtaudienceValue").val(response.data.name);
        }
    })
}

function GetUserName(userId) {
    $.get("/Admin/Notice/GetUser?id=" + userId, function (response) {
        if (response.result) {
            $("#txtaudienceValue").val(response.data.firstName + " " + response.data.lastName);
        }
    })
}

$(document).on('change', '#NoticeType', function () {
    var noticeTypeName = $("#NoticeType option:selected").text();

    if (noticeTypeName === "Announcement") {
        $("#DeliveryMode").val(1);
        $("#DeliveryMode").prop("disabled", true);
    }
});

$("#btnSubmit").on("click", function () {
    $("#DeliveryMode").prop("disabled", false);
})