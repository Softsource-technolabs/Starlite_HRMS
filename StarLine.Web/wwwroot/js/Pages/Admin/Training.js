var $trainingTable = $('#tblTrainingList');
$(function () {
    $trainingTable.DataTable();
})

$(document).on('change', '.toggle-status', function () {
    var id = $(this).data('id');
    $.ajax({
        url: $trainingTable.data('toggleStatus') + "?id=" + id,
        method: 'GET',
        success: function (response) {
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
                var url = $trainingTable.data('delete') + "?id=" + id;
                $.ajax({
                    url: url,
                    type: 'DELETE',
                    success: function (response) {
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
