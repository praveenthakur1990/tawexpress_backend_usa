$(document).ready(function () {
    $('#frmAddUpdateCategory').validate({
        rules: {
            name: {
                required: true,
                maxlength: 50
            },
            description: {
                maxlength: 1000
            },
            bannerPath: {
                required: function () {
                    return $("#ParentId").val() > 0 ? false: $("#hdnBannerImgPath").val() == '' ? true : false;
                }
            }
        }
    });
});

$("#btnSaveCategory").click(function () {
    addUpdateCategorySubmit();
})

function addUpdateCategorySubmit() {
    var file = $("#linkUploadFilePath").find('img').attr('src');
    var bannerFile = $("#linkUploadBannerFilePath").find('img').attr('src');
    if ($("#frmAddUpdateCategory").valid()) {
        var formData = new FormData();
        formData.append("Id", $("#Id").val())
        formData.append("Name", $("#name").val());
        formData.append("Description", $("#description").val());
        formData.append("UploadFile", file);
        formData.append("UploadBannerFile", bannerFile);
        formData.append("hdnUploadFile", $("#hdnImagePath").val());
        formData.append("hdnBannerUploadFile", $("#hdnBannerImgPath").val());
        formData.append("PriorityIndex", $("#PriorityIndex").val());
        formData.append("ParentId", $("#ParentId").val());
        $.ajax({
            url: "/Admin/Category/AddUpdate",
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
                console.log(result)
                var location = '/Admin/Category/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Category have been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Category have been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Category, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                if (jqXHR.status == 401) {
                    toastr.error("Your session has been expired.", "Error", { timeOut: 3000 });
                    refreshPageCustom('/Account/Login', 2000);
                }
                else {
                    toastr.error("An error occured while saving Category, please try again later", "Error", { timeOut: 3000 });
                }
            }
        });
    }

}

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
                toastr.error("Only .pdf file are allowed", "Error", { timeOut: 500 })
                $("#imagePath").val('');
                $("div.preview-image-FSSAI").css('display', 'none');
            }
        }
        else {
            $("#imagePath").val('');
            $("div.preview-image-FSSAI").css('display', 'none');
        }
    }

function readBannerImage(ele) {
        if (ele.files && ele.files[0]) {
            const file = Math.round((ele.files[0].size / 1024));
            if (ele.files[0].type === "image/png" || ele.files[0].type === "image/jpeg" || ele.files[0].type === "image/jpg") {
                // The size of the file. 
                if (file > 2000) {
                    toastr.error("File too Big, please select a file less than 2 MB", "Error", { timeOut: 1000 });
                    $("#bannerPath").val('');
                    $("div.preview-banner-image").css('display', 'none');
                }
                else {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('#linkUploadBannerFilePath').find('img').attr('src', e.target.result);
                    }
                    reader.readAsDataURL(ele.files[0]);
                    $("div.preview-banner-image").css('display', 'block');
                }
            }
            else {
                toastr.error("Only .pdf file are allowed", "Error", { timeOut: 500 })
                $("#bannerPath").val('');
                $("div.preview-banner-image").css('display', 'none');
            }
        }
        else {
            $("#bannerPath").val('');
            $("div.preview-banner-image").css('display', 'none');
        }
    }

function markAsDeleted(categoryId) {
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
                        url: "/Admin/Category/MarkAsDeleted",
                        method: "POST",
                        data: { 'id': categoryId },
                        beforeSend: function () {
                            ajaxindicatorstart(returnLoadingText());
                        },
                        success: function (result) {
                            ajaxindicatorstop();
                            var location = '/Admin/Category/Index';
                            if (parseInt(result) === 1) {
                                toastr.success("Category have been deleted successfully !", "Success", { timeOut: 3000 });
                                refreshPageCustom(location, 1000);
                            }
                            else {
                                toastr.error("An error occured while deleting Category, please try again later", "Error", { timeOut: 3000 });
                            }
                        },
                        error: function (jqXHR) {
                            ajaxindicatorstop();
                            toastr.error("An error occured while deleting Category, please try again later", "Error", { timeOut: 3000 });
                        }
                    });
                } else {
                    return false;
                }
            });
    }