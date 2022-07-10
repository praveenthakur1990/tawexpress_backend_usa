$(document).ready(function () {
    $('#frmAddUpdatePlan').validate({ // initialize the plugin
        rules: {
            Name: {
                required: true,
                maxlength: 50
            },
            Price: {
                required: true,
                maxlength: 10
            },
            Interval: {
                required: true
            },

        }
    });
});

function addUpdatePlan() {
    if ($("#frmAddUpdatePlan").valid()) {
        $.ajax({
            url: "/SecurePanel/Plan/AddUpdatePlan",
            method: "POST",
            data: $("#frmAddUpdatePlan").serialize(),
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/SecurePanel/Plan/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Plan have been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Plan have been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Plan, please try again later", "Error", { timeOut: 3000 });
                }

            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving Plan, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}
