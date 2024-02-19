var dataTable;

$(document).ready(function () {
    var urlParams = new URLSearchParams(window.location.search);

    var statusValue = urlParams.get('status');

    loadDataTable(statusValue);
});

function loadDataTable(status) {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: `/admin/order/getall?status=${status}` },
        columns: [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "10%" },
            {data:'orderTotal',"width": "10%"},
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group d-flex justify-content-center w-50 mx-auto" role="group">  
                               <a href="/admin/order/details?OrderId=${data}&arrombado=arrombado" class="btn btn-primary"><i class="bi bi-pencil-square"></i></a>
                            </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

