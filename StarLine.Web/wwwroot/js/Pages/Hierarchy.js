$(function () {
    initHierarchy();
});

function AddValidation() {
    $("#frmHierarchy").validate({
        rules: {
            levelnumber: { required: true, min: 1 },
            levelname: { required: true, minlength: 2, maxlength: 20 }
        },
        messages: {
            levelnumber: { required: "Enter level as number", min: "Select level minimum as 1" },
            levelname: { required: "Enter level Name", minlength: "Level Name at least 2 characters", maxlength: "Level Name max 20 characters" }
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

async function initHierarchy() {
    await getHierarchy();
    // Handle "Add New Hierarchy" selection
    $(document).on("change", "#HierarchyLevel", async function () {
        const level = $(this).val();
        if (level == "999") { // special "Add New"
            const hierarchyURL = $("#hfaddhierarchy").val();
            const response = await $.get(hierarchyURL);
            $("#hrmsModalBody").html(response);
            $("#hrmsSmallModel").modal("show");
            AddValidation();
        }
    });

    // Handle modal form submit
    $(document).on("click", "#btnModelSubmit", async function () {
        if ($("#frmHierarchy").valid()) {
            const data = new FormData();
            const newlyInsertlevel = $("#levelnumber").val();
            data.append("Id", newlyInsertlevel);
            data.append("Name", $("#levelname").val());
            const url = $("#hierarchyURL").val();

            try {
                const response = await $.ajax({
                    url: url,
                    type: 'POST',
                    data: data,
                    contentType: false,
                    processData: false
                });

                if (response.result > 0) {
                    toastr.success(response.message);
                    $("#hrmsSmallModel").modal("hide");
                    // Refresh dropdown and select newly added value
                    await getHierarchy(newlyInsertlevel);
                } else {
                    toastr.error(response.message);
                }
            } catch (error) {
                console.error("AJAX Error:", error);
            }
        }
    });
}

// Fetch hierarchy and populate dropdown
async function getHierarchy(selectValue = null) {
    try {
        const url = $("#hfgethierarchy").val();
        const response = await $.get(url);
        const dropdown = $('#HierarchyLevel');
        dropdown.empty();
       
        response.forEach(item => {
            dropdown.append(`<option value="${item.value}">${item.text}</option>`);
        });

        // Select newly added value if provided
        if (selectValue) {
            dropdown.val(selectValue).trigger('change');
        }
        if (hierarcyId != "" || hirerchyId != null)
            dropdown.val(hierarcyId).trigger('change');
    } catch (error) {
        console.error("Error loading hierarchy:", error);
    }
}
