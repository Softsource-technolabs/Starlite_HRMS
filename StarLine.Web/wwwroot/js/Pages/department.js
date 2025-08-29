var $departmentTable;

document.addEventListener('DOMContentLoaded', function () {
    $departmentTable = $("#tblDepartmentList").dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": true,
        drawCallback: function () {
            $('.toggle-status').bootstrapToggle();
        },
        "ajax": {
            "url": $('#tblDepartmentList').data('url'),
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
            { "data": "departmentName", "name": "departmentName", "autoWidth": true, orderable: false, },
            { "data": "description", "name": "description", "autoWidth": true },
            {
                data: "isActive", // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="toggle-status" data-id="${row.id}" ${row.isActive ? 'checked' : ''}
                        data-toggle="toggle" data-on="Active" data-off="Inactive" data-onstyle="success" data-offstyle="danger">`;
                }
            },
            {
                data: null, // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return '<a class="btn btn-sm btn-outline-secondary" href="/Admin/Department/ManageDepartment?id=' + data.id + '"><i class="fa-solid fa-pen-to-square"></i></a> '
                        + '<a class="btn btn-sm btn-outline-danger" id="btn-Delete" data-id=' + data.id + '><i class="fa-solid fa-trash"></i></a>';
                }
            }
        ]
    });

    $('.toggle-status').bootstrapToggle();
});

$(document).on('change', '.toggle-status', function () {
    var id = $(this).data('id');
    $.ajax({
        url: $('#tblDepartmentList').data('togglestatus') + "?id=" + id,
        method: 'GET',
        success: function (response) {
            if (response == true) {
                toastr.success("Activation status changed");
            }
            else {
                toastr.error("Activation status not changed");
            }
            $departmentTable.DataTable().ajax.reload(); // Reload the table without resetting pagination
        }
    });
});

$(document).on('click', '#btn-Delete', function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    $.confirm({
        theme: 'material',
        type: 'red',
        animationBounce: 1.5,
        title: 'Confirm!',
        content: 'Are you sure you want to delete this?',
        buttons: {
            confirm: function () {
                debugger;
                var url = $('#tblDepartmentList').data('delete') + "?id=" + id;;
                $.ajax({
                    url: url,
                    type: 'DELETE',
                    success: function (response) {
                        if (response == true) {
                            $departmentTable.DataTable().ajax.reload(); // Reload the table without resetting pagination
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error('Error deleting the record!');
                    }
                });
            },
            cancel: function () {
                toastr.warning('Delete event cancelled!');
            }
        }
    });
})