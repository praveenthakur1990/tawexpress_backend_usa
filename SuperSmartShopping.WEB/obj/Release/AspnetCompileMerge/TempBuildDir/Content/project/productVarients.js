
$(document).ready(function () {
    $("input.switch").bootstrapSwitch({
        onText: 'Yes',
        offText: 'No',
        onColor: 'success',
        offColor: 'danger',
        size: 'small'
    });
    $('#frmAddUpdateProductVarient').validate({
        rules: {
            ProductId: {
                required: true
            },
            name: {
                required: true,
                maxlength: 50
            },
            Price: {
                required: true
            }
        }
    });

    $('input[name="IsPublish"]').on('switchChange.bootstrapSwitch', function (e, data) {      
        $("#hdnisPublished").val(data);
    });   
});

function readImage(ele) {
    if (ele.files && ele.files[0]) {
        const file = Math.round((ele.files[0].size / 1024));
        if (ele.files[0].type === "image/png" || ele.files[0].type === "image/jpeg" || ele.files[0].type === "image/jpg") {
            // The size of the file. 
            if (file > 2000) {
                toastr.error("File too Big, please select a file less than 2 MB", "Error", { timeOut: 1000 });
                $("#imagePath").val('');
                $("div.preview-image-FSSAI").css('display', 'none');
            }
            else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#linkUploadFilePath').find('img').attr('src', e.target.result);
                }
                reader.readAsDataURL(ele.files[0]);
                $("div.preview-image-FSSAI").css('display', 'block');
            }
        }
        else {
            toastr.error("Only .png, .jpeg, .jpg files are allowed", "Error", { timeOut: 500 })
            $("#imagePath").val('');
            $("div.preview-image-FSSAI").css('display', 'none');
        }
    }
    else {
        $("#imagePath").val('');
        $("div.preview-image-FSSAI").css('display', 'none');
    }
}

function addUpdateProductVarient() {
    if ($("#frmAddUpdateProductVarient").valid()) {
        var Imagefile = $("#linkUploadFilePath").find('img').attr('src');
        var formData = new FormData();
        formData.append("Id", $("#Id").val())
        formData.append("ProductId", $("#ProductId").val());
        formData.append("Name", $("#name").val());
        formData.append("Price", $("#Price").val());
        formData.append("ImagePath", Imagefile);
        formData.append("IsPublish", $("#hdnisPublished").val());
        formData.append("hdnUploadFile", $("#hdnImagePath").val());
        $.ajax({
            url: "/Admin/ProductVarient/AddUpdate",
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
                var location = '/Admin/ProductVarient/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Product varient has been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Product varient has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Product varient, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving Product varient, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function markAsDeleted(menuId) {
    swal({
        title: "",
        text: "Are you sure you want to delete?",
        icon: "warning",
        buttons: ["No", "Yes"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: "/Admin/Product/MarkAsDeleted",
                    method: "POST",
                    data: { 'id': menuId },
                    beforeSend: function () {
                        ajaxindicatorstart(returnLoadingText());
                    },
                    success: function (result) {
                        ajaxindicatorstop();
                        var location = '/Admin/Menu/Index';
                        if (parseInt(result) === 1) {
                            toastr.success("Product has been deleted successfully !", "Success", { timeOut: 3000 });
                            refreshPageCustom(location, 1000);
                        }
                        else {
                            toastr.error("An error occured while deleting Product, please try again later", "Error", { timeOut: 3000 });
                        }
                    },
                    error: function (jqXHR) {
                        ajaxindicatorstop();
                        toastr.error("An error occured while deleting Product, please try again later", "Error", { timeOut: 3000 });
                    }
                });
            } else {
                return false;
            }
        });
}

function bindSubcategories(ele) {
    if ($(ele).val() > 0) {
        $.ajax({
            type: "POST",
            url: "/Admin/Product/GetSubCategories",
            data: { 'id': $(ele).val() },
            dataType: "json",
            success: function (data) {
                var optionhtml1 = '';
                $("select#SubCategoryId").empty();
                if (data.length > 0) {
                    optionhtml1 = '<option value="' +
                        0 + '">' + "--Select--" + '</option>';
                    $("select#SubCategoryId").append(optionhtml1);
                    $.each(data, function (i) {
                        var optionhtml = '<option value="' +
                            data[i].Value + '">' + data[i].Text + '</option>';
                        $("select#SubCategoryId").append(optionhtml);
                    });
                }
                else {
                    optionhtml1 = '<option value="' +
                        0 + '">' + "No Sub-category found" + '</option>';
                    $("select#SubCategoryId").append(optionhtml1);
                }
            },
            error: function () {
                console.log("error")
            }
        });
    }
    else {
        $("select#SubCategoryId").empty();
        var optionhtml2 = '<option value="' +
            0 + '">' + "--Select--" + '</option>';
        $("select#SubCategoryId").append(optionhtml2);
    }

}

function btnGetProductSearch() {
    window.location.href = '/admin/Product/Index?page=1&searchStr=' + $("#txtSearch").val() + '';
}