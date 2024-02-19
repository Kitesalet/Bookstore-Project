var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/admin/product/getall' },
        columns: [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'price', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">  
                            <a href="/admin/product/upsert?id=${data}&arrombado=arrombado" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                            <a onClick=Delete('/admin/product/delete?id=${data}&arrombado=arrombado') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i></a>
                            </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure you want to delete this product?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
                error: function (data) {
                    toastr.error("An error occurred while deleting the product.");
                }
            });
        }
    });
}