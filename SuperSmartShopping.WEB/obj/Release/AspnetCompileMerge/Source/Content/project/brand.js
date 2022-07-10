$(document).ready(function () {
    $('#frmAddUpdateBrand').validate({
        rules: {
            name: {
                required: true,
                maxlength: 100
            }
        }
    });
});


function addUpdate() {
    if ($("#frmAddUpdateBrand").valid()) {     
        $.ajax({
            url: "/Admin/Brand/AddUpdate",
            method: "POST",
            data: $("#frmAddUpdateBrand").serialize(),
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/Admin/Brand/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Brand has been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Brand has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving brand, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving brand, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }

}

function btnGetBrandSearch() {
    window.location.href = '/admin/Brand/Index?page=1&searchStr=' + $("#txtSearch").val() + '';
}