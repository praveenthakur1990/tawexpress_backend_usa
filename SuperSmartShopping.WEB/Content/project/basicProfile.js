var base64Arr = [];
$(document).ready(function () {
    $('#frmAddUpdateBannerImages').validate({
        rules: {
            bannerImgPath: {
                required: true
            }
        }
    });
});

function readImage(ele) {
    if (ele.files.length > 5) {
        toastr.error("Images should not be greater than 5", "Error", { timeOut: 500 });
        $("#bannerImgPath").val('');
    }
    else {
        for (let i = 0; i < ele.files.length; i++) {
            const file = Math.round((ele.files[i].size / 1024));
            if (ele.files[i].type === "image/png" || ele.files[i].type === "image/jpeg" || ele.files[i].type === "image/jpg") {
                // The size of the file. 
                if (file > 3000) {
                    toastr.error(ele.files[i].name + " file is too Big, please select a file less than 3 MB", "Error", { timeOut: 1000 });
                    // $("div.preview-image-logo").css('display', 'none');
                }
                else {
                    $('div.preview-image').show();
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        base64Arr.push(e.target.result);
                        console.log(base64Arr)

                        $('div.preview-image').append('<img src="' + e.target.result + '" />');
                        // $('#linkUploadFilePath').find('img').attr('src', e.target.result);
                    }
                    reader.readAsDataURL(ele.files[i]);
                    // console.log(ele.files[i])
                    // $("div.preview-image").css('display', 'block');
                }
            }
            else {
                toastr.error("Only .png, .jpeg, .jpg file are allowed", "Error", { timeOut: 500 })
                // $("#logoImagePath").val('');
                // $("div.preview-image-logo").css('display', 'none');
            }
        }
        console.log(base64Arr.length)
    }
}

function openBannerModel(ele) {
    var _url = $(ele).attr('data-bind');
    $.ajax({
        url: _url == '' || _url == undefined ? "/Admin/Setting/OpenBannerImage" : _url,
        method: "GET",
        beforeSend: function () {
            ajaxindicatorstart(returnLoadingText());
        },
        success: function (result) {
            ajaxindicatorstop();
            $("#addBannerModel").modal('show');
            $("#addBannerModel").find('div.modal-body').html(result);
            $(document).ready(function () {
                $('#frmAddUpdateBannerImages').validate({
                    rules: {
                        bannerImgPath: {
                            required: true
                        }
                    }
                });
            });
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

function saveImages(ele) {
    var _url = $(ele).attr('data-bind');
    var _refreshUrl = $(ele).attr('data-content');
    var _userId = getUrlVars()["userId"];
    var imageArr = [];
    if ($('#frmAddUpdateBannerImages').valid()) {
        $("div.preview-image img").each(function () {
            imageArr.push($(this).attr('src'))
        })
        if (imageArr.length > 0) {
            $.ajax({
                url: _url == '' || _url == undefined ? "/Admin/Setting/AddUpdateBannerImage" : _url,
                method: "POST",
                data: { 'bannerImages': imageArr, 'storeId': _userId },
                traditional: true,
                beforeSend: function () {
                    ajaxindicatorstart(returnLoadingText());
                },
                success: function (result) {
                    ajaxindicatorstop();
                    console.log(result)
                    var location = _refreshUrl == '' || _refreshUrl == undefined ? '/Admin/Setting/Index' : _refreshUrl + '?userId=' + _userId;
                    if (parseInt(result) === 1) {
                        toastr.success("Banner Images has been added successfully !", "Success", { timeOut: 3000 });
                        refreshPageCustom(location, 1000);
                    }
                    else if (parseInt(result) === -1) {
                        toastr.error("Please add atleast one image", "Error", { timeOut: 3000 });
                    }
                    else {
                        toastr.error("An error occured while saving images, please try again later", "Error", { timeOut: 3000 });
                    }
                },
                error: function (jqXHR) {
                    ajaxindicatorstop();
                    if (jqXHR.status == 401) {
                        toastr.error("Your session has been expired.", "Error", { timeOut: 3000 });
                        refreshPageCustom('/Account/Login', 2000);
                    }
                    else {
                        toastr.error("An error occured while saving saving images, please try again later", "Error", { timeOut: 3000 });
                    }
                }
            });
        }
        else {
            toastr.error("Please add atleast one image", "Error", { timeOut: 3000 });
        }
    }
    else {
        // return false;
    }
}

function changeActiveStatus(ele, id, status) {
    var _url = $(ele).attr('data-bind');
    var _refreshUrl = $(ele).attr('data-content');
    var _userId = getUrlVars()["userId"];
    swal({
        title: "",
        text: 'Are you sure you want to mark as ' + (status == 'true' ? "In-Active" : "Active") + "?",
        icon: "warning",
        buttons: ["No", "Yes"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: _url == '' || _url == undefined ? "/Admin/Setting/MarkAsActiveInActive" : _url,
                    method: "POST",
                    data: { 'id': id, 'status': status, 'storeId': _userId  },
                    beforeSend: function () {
                        ajaxindicatorstart(returnLoadingText());
                    },
                    success: function (result) {
                        ajaxindicatorstop();
                        var location = _refreshUrl == '' || _refreshUrl == undefined ? '/Admin/Setting/Index' : _refreshUrl + '?userId=' + _userId;
                        if (parseInt(result) === 1) {
                            toastr.success("Status has been changed successfully !", "Success", { timeOut: 3000 });
                            refreshPageCustom(location, 1000);
                        }
                        else {
                            toastr.error("An error occured while changing status, please try again later", "Error", { timeOut: 3000 });
                        }
                    },
                    error: function (jqXHR) {
                        ajaxindicatorstop();
                        toastr.error("An error occured while changing status, please try again later", "Error", { timeOut: 3000 });
                    }
                });
            } else {
                return false;
            }
        });
}

function markAsDeleted(ele, id) {
      var _url = $(ele).attr('data-bind');
    var _refreshUrl = $(ele).attr('data-content');
    var _userId = getUrlVars()["userId"];  
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
                    url: _url == '' || _url == undefined ? "/Admin/Setting/MarkAsDelete" : _url,
                    method: "POST",
                    data: { 'id': id, 'storeId': _userId  },
                    beforeSend: function () {
                        ajaxindicatorstart(returnLoadingText());
                    },
                    success: function (result) {
                        ajaxindicatorstop();
                        var location = _refreshUrl == '' || _refreshUrl == undefined ? '/Admin/Setting/Index' : _refreshUrl + '?userId=' + _userId;
                        if (parseInt(result) === 1) {
                            toastr.success("Banner has been deleted successfully !", "Success", { timeOut: 3000 });
                            refreshPageCustom(location, 1000);
                        }
                        else {
                            toastr.error("An error occured while deleting banner, please try again later", "Error", { timeOut: 3000 });
                        }
                    },
                    error: function (jqXHR) {
                        ajaxindicatorstop();
                        toastr.error("An error occured while deleting banner, please try again later", "Error", { timeOut: 3000 });
                    }
                });
            } else {
                return false;
            }
        });
}

