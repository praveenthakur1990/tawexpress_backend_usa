$(document).ready(function () {
    $('#frmAddUpdateUnitMeasurement').validate({
        rules: {
            name: {
                required: true,
                maxlength: 50
            },
            shortName: {
                required: true,
                maxlength: 10
            }
        }
    });
});


function addUpdate() {
    if ($("#frmAddUpdateUnitMeasurement").valid()) {
        $.ajax({
            url: "/Admin/UnitMeasurement/AddUpdate",
            method: "POST",
            data: $("#frmAddUpdateUnitMeasurement").serialize(),
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/Admin/UnitMeasurement/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Unit Measurement has been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Unit Measurement has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === -2) {
                    toastr.error("Name already exists!", "Error", { timeOut: 3000 });                    
                }
                else {
                    toastr.error("An error occured while saving UnitMeasurement, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving UnitMeasurement, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }

}

