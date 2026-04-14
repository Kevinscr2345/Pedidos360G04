// DataTables initialization for Pedidos table
$(document).ready(function () {
    if ($.fn.dataTable) {
        $('#pedidosTable').DataTable({
            responsive: true,
            pageLength: 10,
            lengthMenu: [5, 10, 25, 50],
            columnDefs: [
                { targets: -1, orderable: false }
            ],
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
            }
        });
    }
});
