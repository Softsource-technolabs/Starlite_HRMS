var $SessionTable;
$(function () {
    $SessionTable = $("#tblTrainingSession").dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": true,
        drawCallback: function () {
            $('.toggle-status').bootstrapToggle();
        },
        "ajax": {
            "url": $("#tblTrainingSession").data('url'),
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
            { "data": "trainingCode", "name": "trainingCode", "autoWidth": true, orderable: false, },
            { "data": "trainingName", "name": "trainingName", "autoWidth": true },
            { "data": "category", "name": "category", "autoWidth": true },
            {
                data: "startDate", // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return formatDate(row.startDate);
                }
            },
            {
                data: "endDate", // Important: set data to null for custom HTML
                orderable: false,
                searchable: false,
                className: 'text-center',
                "render": function (data, type, row) {
                    return row.endDate != null ? formatDate(row.endDate) : "";
                }
            },
            { "data": "location", "name": "location", "autoWidth": true },
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
                    return '<a class="btn btn-sm btn-outline-secondary" href="/Admin/TrainingSession/ManageSession?id=' + data.id + '"><i class="fa-solid fa-pen-to-square"></i></a>';
                }
            }
        ]
    });

    $('.toggle-status').bootstrapToggle();
})

$(document).on('change', '.toggle-status', function () {
    var id = $(this).data('id');
    $.ajax({
        url: $SessionTable.data('togglestatus') + "?id=" + id,
        method: 'GET',
        success: function (response) {
        }
    });
});