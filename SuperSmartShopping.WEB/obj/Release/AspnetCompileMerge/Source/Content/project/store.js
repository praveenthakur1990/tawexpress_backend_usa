$(document).ready(function () {
    $('#frmAddUpdateStore').validate({ // initialize the plugin
        rules: {
            name: {
                required: true,
                maxlength: 50
            },
            subDomainName: {
                required: true,
                maxlength: 50
            },
            emailAddress: {
                required: true,
                maxlength: 50,
                customEmail: true
            },
            mobile: {
                required: true,
                maxlength: 15
            },
            address: {
                required: true,
                maxlength: 200
            },
            state: {
                required: true
            },
            city: {
                required: true,
                maxlength: 50
            },
            zipCode: {
                required: true,
                maxlength: 6
            },
            countryCode: {
                required: true,
                maxlength: 4
            },
            CurrencySymbol: {
                required: true,
                maxlength: 1
            },
            timeZone: {
                required: true
            },
            contactPersonName: {
                required: true,
                maxlength: 50
            },
            contactNumber: {
                required: true,
                maxlength: 15
            },
            logoImagePath: {
                required: $("#hdnLogoFilePath").val().length <= 0 ? true : false
            },
            GST: {
                required: false,
                maxlength: 50
            },
            activePlan: {
                required: true
            },
            PlanId: {
                required: true
            },
            commision: {
                required: true,
                maxlength: 5
            },
            planActiveDate: {
                required: true
            }

        }
    });
    jQuery.validator.addMethod("customEmail", function (value, element) {
        // allow any non-whitespace characters as the host part
        var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
        return this.optional(element) || pattern.test(value);
    }, 'Please enter a valid email address.');

    $(function () {
        var options = {
            //types: ['(cities)'],
            componentRestrictions: {
                country: ["US", "IN", "ES"]
            }
        };
        var places = new google.maps.places.Autocomplete(document.getElementById('address'), options);
        google.maps.event.addListener(places, 'place_changed', function () {
            var result = places.getPlace();
            var state_shortname = "";
            var state_longname = "";
            var city = "";
            var zipCode = "";
            var country = '';
            var address_components = result.address_components;
            $.each(address_components, function (index, component) {
                var types = component.types;
                console.log(component)
                $.each(types, function (index, type) {
                    if (type == 'locality') {
                        city = component.long_name;
                    }
                    if (type == 'administrative_area_level_1') {
                        state_shortname = component.short_name;
                        state_longname = component.long_name;
                    }
                    if (type == 'postal_code') {
                        zipCode = component.long_name;
                    }
                    if (type == 'country') {
                        country = component.short_name
                    }
                });
            });

            console.log("address:" + result.formatted_address)
            console.log("state_shortname:" + state_shortname)
            console.log("state_longname:" + state_longname)
            console.log("City:" + city)
            console.log("zipCode:" + zipCode)
            console.log("country:" + country)
            console.log("lat:" + result.geometry.location.lat())
            console.log("long:" + result.geometry.location.lng())

            $("#city").val(city);
            $("#state").val(state_longname);
            $("#zipCode").val(zipCode);
            $("#countryCode").val(country);
            $("#hdnLatitude").val(result.geometry.location.lat());
            $("#hdnLongitude").val(result.geometry.location.lng());
        });
    })

});

$("#btnSave").click(function () {
    var GSTFile = $("#linkGSTFilePath").attr('href');
    var LogoFile = $("#linkUploadFilePath").find('img').attr('src');
    if ($("#frmAddUpdateStore").valid()) {
        var formData = new FormData();
        formData.append("Id", $("#Id").val())
        formData.append("Name", $("#name").val());
        if ($("#Id").val() == 0) {
            formData.append("subDomainName", $("#subDomainName").val());
            formData.append("Email", $("#emailAddress").val());
        }
        else {
            formData.append("Email", $("#hdnEmailAddress").val());
            formData.append("subDomainName", $("#hdnSubDomain").val());
        }

        formData.append("Mobile", $("#mobile").val());
        formData.append("Address", $("#address").val());
        formData.append("State", $("#state").val());
        formData.append("City", $("#city").val());
        formData.append("ZipCode", $("#zipCode").val());
        formData.append("CountryCode", $("#countryCode").val());
        formData.append("CurrencySymbol", $("#CurrencySymbol").val());
        formData.append("Latitude", $("#hdnLatitude").val());
        formData.append("Longitude", $("#hdnLongitude").val());
        formData.append("ContactPersonName", $("#contactPersonName").val());
        formData.append("ContactNumber", $("#contactNumber").val());
        formData.append("GSTRegistrationNumber", $("#GST").val());
        formData.append("ActivePlan", $("#activePlan").val());
        formData.append("Commision", $("#commision").val());
        formData.append("PlanActiveDate", $("#planActiveDate").val());
        formData.append("TimeZone", $("#timeZone").val());
        formData.append("GSTFile", GSTFile);
        formData.append("LogoFile", LogoFile);
        formData.append("hdnGSTFile", $("#hdnGSTFilePath").val());
        formData.append("hdnLogoFile", $("#hdnLogoFilePath").val());
        formData.append("PlanId", $("#PlanId").val());
        formData.append("QrCodePath", $("#hdnQrCodePath").val());
        
        $.ajax({
            url: "/SecurePanel/Store/AddUpdate",
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
                var location = '/SecurePanel/Store/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Store has been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Store has been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Store, please try again later", "Error", { timeOut: 3000 });
                }

            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                debugger
                toastr.error("An error occured while saving Store, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
})

$('body').delegate("#UploadGST", 'change', function () {
    readURLInvoice(this, 'GST');
});

function readURLInvoice(input, type) {
    if (input.files && input.files[0]) {
        const file = Math.round((input.files[0].size / 1024));
        if (input.files[0].type === "application/pdf") {
            // The size of the file. 
            if (file > 2000) {
                toastr.error("File too Big, please select a file less than 2 MB", "Error", { timeOut: 1000 });
                $("#UploadGST").val('');
            }
            else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#linkGSTFilePath').attr('href', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
        else {
            toastr.error("Only .pdf file are allowed", "Error", { timeOut: 500 })
            $("#UploadGST").val('');
        }
    }
    else {
        $("#UploadGST").val('');
    }
}

function readImage(ele) {
    if (ele.files && ele.files[0]) {
        const file = Math.round((ele.files[0].size / 1024));
        if (ele.files[0].type === "image/png" || ele.files[0].type === "image/jpeg" || ele.files[0].type === "image/jpg") {
            // The size of the file. 
            if (file > 3000) {
                toastr.error("File too Big, please select a file less than 3 MB", "Error", { timeOut: 1000 });
                $("#logoImagePath").val('');
                $("div.preview-image-logo").css('display', 'none');
            }
            else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#linkUploadFilePath').find('img').attr('src', e.target.result);
                }
                reader.readAsDataURL(ele.files[0]);
                $("div.preview-image-logo").css('display', 'block');
            }
        }
        else {
            toastr.error("Only .png, .jpeg, .jpg file are allowed", "Error", { timeOut: 500 })
            $("#logoImagePath").val('');
            $("div.preview-image-logo").css('display', 'none');
        }
    }
    else {
        $("#logoImagePath").val('');
        $("div.preview-image-logo").css('display', 'none');
    }
}

function getPlanCost() {
    var element = $("select#PlanName").find('option:selected');
    var myTag = element.attr("data-content");
    return myTag;
}

function getPlanCostSection(ele) {
    if ($(ele).find('option:selected').text() == 'Paid') {
        $("#PlanAndCostDiv").show();
    }
    else {
        $("#PlanAndCostDiv").hide();
    }
}