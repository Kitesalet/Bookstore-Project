

$(document).ready(function () {
    startDataTable();
});

const startDataTable = () => {

       dataTable = $('#myTable').DataTable({
            "ajax": { url: '/admin/company/getcompanies' },
            columns: [
                { data: 'name', width:"10%"},
                { data: 'streetAddress', width:"15%"},
                { data: 'city', width:"10%"},
                { data: 'state', width:"10%"},
                { data: 'postalCode', width:"15%" },
                { data: 'phoneNumber' ,width: '15%'},
                {
                    data: 'id',
                    "render": function (data) {
                        return `<div class="w-75 btn-group" role="group">  
                            <a href="/admin/company/upsert?id=${data}&arrombado=arrombado" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                            <a onClick=Delete('/admin/company/delete?id=${data}&arrombado=arrombado') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i></a>
                            </div>`;
                    },
                    "width": "25%"
                }
            ]
       });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure you want to delete this company?",
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
                    console.log(data)
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

