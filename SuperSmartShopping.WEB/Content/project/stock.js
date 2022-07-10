
$(document).ready(function () {
    $('#frmAddStock').validate({
        rules: {
            Quantity: {
                required: true
            }
        }
    });
});

function addStock() {
    if ($("#frmAddStock").valid()) {
        $.ajax({
            url: "/Admin/Inventory/AddStock",
            method: "POST",
            data: $("#frmAddStock").serialize(),
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/Admin/Inventory/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Stock has been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Stock has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while adding stock, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while adding stock, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}
