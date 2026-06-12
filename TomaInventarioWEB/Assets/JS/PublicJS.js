$(document).ready(function () {
    $('#RImport_modal').on('hidden.bs.modal', function () {
        // Destruir DataTable cuando se oculta el modal para la recarga de la tabla
        var tb_Import = $('#RImport_Tbl').DataTable();
        tb_Import.destroy();
    });

});
function toggle_Loadingtb(idLoading, mostrar) {
    if (mostrar) {
        $('#' + idLoading).css("visibility", "visible");
    } else { $('#' + idLoading).css("visibility", "hidden"); }

}