// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatUTC(input) {
    const d = new Date(input);
    const mm = String(d.getUTCMonth() + 1).padStart(2, '0');
    const dd = String(d.getUTCDate()).padStart(2, '0');
    const yyyy = d.getUTCFullYear();
    return `${mm}/${dd}/${yyyy}`;
}

$.validator.addMethod("requiredif", function (value, element, params) {
    var dependentProperty = params["dependentproperty"];
    var targetValue = params["targetvalue"];

    var dependentElement = $("[name='" + dependentProperty + "']");
    var dependentValue = dependentElement.val();

    if (dependentValue === targetValue) {
        return $.trim(value).length > 0; // must be filled
    }
    return true; // not required
});

$.validator.unobtrusive.adapters.add("requiredif", ["dependentproperty", "targetvalue"], function (options) {
    options.rules["requiredif"] = {
        dependentproperty: options.params.dependentproperty,
        targetvalue: options.params.targetvalue
    };
    options.messages["requiredif"] = options.message;
});