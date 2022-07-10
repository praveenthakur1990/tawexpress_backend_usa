$(document).ready(function () {
    $(".chosen-select").chosen();
    var _tagIds = $("#hdnStoreIds").val();
    var _tagIds_array = _tagIds.split(',');
    if (_tagIds_array.length > 0) {
        $(".chosen-select").val(_tagIds_array).trigger("chosen:updated");
    }

    $("input.switch").bootstrapSwitch({
        onText: 'Yes',
        offText: 'No',
        onColor: 'success',
        offColor: 'danger',
        size: 'small'
    });

    $('#frmAddUpdateRider').validate({
        rules: {
            firstName: {
                required: true
            },
            lastName: {
                required: true
            },
            ddlGender: {
                required: true
            },
            mobile: {
                required: true
            },
            emailAddress: {
                required: true,
                maxlength: 200
            },
            contactAddress: {
                required: true,
                maxlength: 100
            },
            city: {
                required: true,
                maxlength: 50
            },
            state: {
                required: true,
                 maxlength: 50
            },
            zipCode: {
                required: true,
                maxlength: 10
            }
        }
    });
});

function addUpdate() {
    var selectedStoreArr = [];
    $('.chosen-select option:selected').each(function (index, valor) {
        selectedStoreArr.push(valor.value);
    });
    if ($("#frmAddUpdateRider").valid()) {
        var formData = new FormData();
        formData.append("Id", $("#Id").val())
        formData.append("firstName", $("#firstName").val());
        formData.append("lastName", $("#lastName").val());
        formData.append("gender", $('#ddlGender option:selected').val());
        formData.append("mobile", $("#mobile").val());
        formData.append("emailAddress", $("#emailAddress").val());
        formData.append("contactAddress", $("#contactAddress").val());
        formData.append("city", $("#city").val());
        formData.append("state", $("#state").val());        
        formData.append("zipCode", $("#zipCode").val());
        formData.append("storeIds", selectedStoreArr.length > 0 ? selectedStoreArr.toString() : '');
        console.log(formData)
        $.ajax({
            url: "/SecurePanel/Rider/AddUpdateRider",
            method: "POST",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/SecurePanel/Rider/Index';
                if (parseInt(result) === 0) {
                    toastr.success("Rider has been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) > 0) {
                    toastr.success("Rider has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error(result, "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving Rider, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function btnGetSearchRider() {
    window.location.href = '/SecurePanel/Rider/Index?page=1&searchStr=' + $("#txtSearch").val() + '';
}