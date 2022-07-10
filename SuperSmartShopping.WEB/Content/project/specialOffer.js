$(document).ready(function () {
    $('#frmAddUpdateSpecialOffer').validate({
        rules: {
            title: {
                required: true,
                maxlength: 100
            },
            startdate: {
                required: true
            },
            enddate: {
                required: true
            },
            bannerImagePath: {
                required: function () {
                    return $("#hdnBannerImagePath").val() == '' ? true : false;
                }
            }
        }
    });
});

function addUpdate() {
    var file = $("img#thumbnailImg").attr('src');
    if ($("#frmAddUpdateSpecialOffer").valid()) {
        var formData = new FormData();
        formData.append("Id", $("#Id").val())
        formData.append("Title", $("#title").val());
        formData.append("StartDate", $("#startdate").val());
        formData.append("EndDate", $("#enddate").val());
        formData.append("BannerImagePath", file);
        formData.append("hdnBannerImagePath", $("#hdnBannerImagePath").val());
        $.ajax({
            url: "/Admin/SpecialOffer/AddUpdate",
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
                var location = '/Admin/SpecialOffer/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Special Offer have been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Special Offer have been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Special Offer, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving Special Offer, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function btnGetSpecialOfferSearch() {
    window.location.href = '/admin/SpecialOffer/Index?page=1&searchStr=' + $("#txtSearch").val() + '';
}

function openAddUpdateProduct(specialOfferId, catId, subCaId, specialOfferCatId) {
    $.ajax({
        type: "POST",
        url: "/Admin/SpecialOffer/GetAddUpdateProduct",
        data: { 'specialOfferId': specialOfferId, 'catId': catId, 'subCatId': subCaId, 'specialOfferCatId': specialOfferCatId },
        dataType: "json",
        beforeSend: function () {
            ajaxindicatorstart(returnLoadingText());
        },
        success: function (data) {
            $("#addWeeklyCircularProducts").modal('show');
            $("#addWeeklyCircularProducts").find('div.modal-body').empty().html(data);
            $(function () {
                if ($("#SubCategoryId").val() > 0) {
                    bindProducts($("#SubCategoryId"));
                }

                $('#frmAddUpdateWeeklyCircularProduct').validate({
                    rules: {
                        categoryId: {
                            required: true
                        },
                        SubCategoryId: {
                            required: true
                        }
                    }
                });
            })
        },
        error: function () {
            console.log("error")
        },
        complete: function () {
            ajaxindicatorstop();
        }
    });
}


function addUpdateProduct() {
    var selectedProductArr = [];
    $('div.panel-group').find('div.selected').each(function () {
        var selectedProduct = {
            'ProductId': parseInt($(this).attr('id')),
            'OfferType': $(this).find('input[type="radio"]:checked').attr('id'),
            'OfferValue': parseFloat($(this).find('div#offerTypeValue div.active input').val())
        }
        selectedProductArr.push(selectedProduct);
    })
    if ($("#frmAddUpdateWeeklyCircularProduct").valid()) {
        if (selectedProductArr.length == 0) {
            toastr.error("Select atleast one product", "Error", { timeOut: 3000 });
            return false;
        }
        var formData = new FormData();
        formData.append("WeeklyCircularCatId", $("#WeeklyCircularCatId").val())
        formData.append("WeeklyCircularId", $("#WeeklyCircularId").val())
        formData.append("categoryId", $("#categoryId").val());
        formData.append("SubCategoryId", $("#SubCategoryId").val());
        formData.append("SelectedProduct", JSON.stringify(selectedProductArr));
        $.ajax({
            url: "/Admin/SpecialOffer/AddUpdateProduct",
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
                var location = '/Admin/SpecialOffer/ViewProduct/' + $("#WeeklyCircularId").val();
                if (parseInt(result) === 1) {
                    toastr.success("Product has been added successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Product has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Special Offer Circular product, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving Special Offer product, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function bindSubcategories(ele) {
    if ($(ele).val() > 0) {
        $.ajax({
            type: "POST",
            url: "/Admin/WeeklyCircular/GetSubCategories",
            data: { 'id': $(ele).val() },
            dataType: "json",
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (data) {
                var optionhtml1 = '';
                $("select#SubCategoryId").empty();
                if (data.length > 0) {
                    optionhtml1 = '<option value="">' + "--Select--" + '</option>';
                    $("select#SubCategoryId").append(optionhtml1);
                    $.each(data, function (i) {
                        var optionhtml = '<option value="' +
                            data[i].Value + '">' + data[i].Text + '</option>';
                        $("select#SubCategoryId").append(optionhtml);
                    });
                }
                else {
                    optionhtml1 = '<option value="">' + "No Sub-category found" + '</option>';
                    $("select#SubCategoryId").append(optionhtml1);
                }
            },
            error: function () {
                console.log("error")
            },
            complete: function () {
                $("#loadProducts").empty();
                ajaxindicatorstop();
            }
        });
    }
    else {
        $("select#SubCategoryId").empty();
        var optionhtml2 = '<option value="">' + "--Select--" + '</option>';
        $("select#SubCategoryId").append(optionhtml2);
    }
}

function bindProducts(ele) {
    if ($(ele).val() > 0) {
        $.ajax({
            type: "POST",
            url: "/Admin/SpecialOffer/GetProductsBySubCategories",
            data: { 'catId': $("select#categoryId").val(), 'subCatId': $(ele).val(), 'weeklyCircularId': $("#WeeklyCircularId").val() },
            dataType: "json",
            success: function (data) {
                $("#loadProducts").empty().html(data);
            },
            error: function () {
                console.log("error")
            }
        });
    }
    else {
        $("#loadProducts").empty();
    }
}

function addOfferType(ele, productId, price) {
    var offerTypeId = $('input[name="offerType_' + productId + '"]:checked').attr('id');
    var offerValue = $(ele).closest('div').parents('div.panel-body').find('div#offerTypeValue div.active input').val();
    var parentEle = $(ele).closest('div').parents('div.panel');
    if (offerValue == '') {
        toastr.error("Please enter the offer value !", "Error", { timeOut: 3000 });
        if (parentEle.hasClass('selected')) {
            parentEle.removeClass('selected');
        }
    }
    else {
        if (parseInt(offerTypeId) == 1 && parseFloat(offerValue) > 100) {
            toastr.error("Percentage should be between 0-100", "Error", { timeOut: 3000 });
            $(ele).closest('div').parents('div.panel-body').find('div#offerTypeValue div.active input').val('');
            return false;
        }
        if (parseInt(offerTypeId) == 2 && parseFloat(offerValue) >= price) {
            toastr.error("Price should be less than actual product price", "Error", { timeOut: 3000 });
            $(ele).closest('div').parents('div.panel-body').find('div#offerTypeValue div.active input').val('');
            return false;
        }
        if (!parentEle.hasClass('selected')) {
            parentEle.addClass('selected');
        }
        toastr.success("Added !", "Success", { timeOut: 1000 });
    }
}

function openValueField(ele, productId, offerTypeId) {
    var divEle = $(ele).closest('div').parents('div.panel-body').find('div#offerTypeValue');
    // divEle.show();
    divEle.find('div').each(function (e) {
        $(this).val('').removeClass('error').removeClass('active').hide();
    })
    divEle.find('div').eq(offerTypeId).addClass('active').show();
}



function readThubnailImage(ele) {
    if (ele.files && ele.files[0]) {
        const file = Math.round((ele.files[0].size / 1024));
        if (ele.files[0].type === "image/jpeg" || ele.files[0].type === "image/png" || ele.files[0].type === "image/jpg") {
            // The size of the file. 
            if (file > 2500) {
                toastr.error("File too Big, please select a file less than 2.5 MB", "Error", { timeOut: 3000 });
                $("#bannerImagePath").val('');
                $("div.preview-thumbnail-image").css('display', 'none');
            }
            else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    // $('#linkUploadFilePath').attr('href', e.target.result);
                    $('img#thumbnailImg').attr('src', e.target.result);
                }
                reader.readAsDataURL(ele.files[0]);
                $("div.preview-thumbnail-image").css('display', 'block');
            }
        }
        else {
            toastr.error("Only .jpeg, .png, .jpg extension files are allowed", "Error", { timeOut: 500 })
            $("#bannerImagePath").val('');
            $("div.preview-thumbnail-image").css('display', 'none');
        }
    }
    else {
        $("#bannerImagePath").val('');
        $("div.preview-thumbnail-image").css('display', 'none');
    }
}