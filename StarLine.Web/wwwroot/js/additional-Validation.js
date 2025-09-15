
$(function () {
    $('input[type="text"]').attr('autocomplete', 'off');
});

$.validator.setDefaults({
    highlight: function (element) {
        $(element).addClass("is-invalid").removeClass("is-valid");
    },
    unhighlight: function (element) {
        $(element).removeClass("is-invalid").addClass("is-valid");
    },
    errorElement: "div",
    errorClass: "invalid-feedback",
    errorPlacement: function (error, element) {
        if (element.parent(".input-group").length) {
            error.insertAfter(element.parent()); // for input-groups
        } else if (element.is(":checkbox")) {
            error.insertAfter(element.next("label")); // for checkboxes
        } else {
            error.insertAfter(element);
        }
    }
});