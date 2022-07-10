$(document).ready(function () {
    $('#frmAddUpdatePartner').validate({ // initialize the plugin
        rules: {
            Name: {
                required: true,
                maxlength: 50
            },
            EmailAddress: {
                required: true,
                maxlength: 100,
                customEmail: true
            },
            ContactNo: {
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
            Country: {
                required: true,
                maxlength: 4
            },
            Commision: {
                required: true,
                maxlength: 6
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
            $("#city").val(city);
            $("#state").val(state_longname);
            $("#zipCode").val(zipCode);
            $("#Country").val(country);
        });
    })

});

function addUpdatePartner() {
    if ($("#frmAddUpdatePartner").valid()) {
        $.ajax({
            url: "/SecurePanel/Partner/AddUpdatePartner",
            method: "POST",
            data: $("#frmAddUpdatePartner").serialize(),
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/SecurePanel/Partner/Index';
                if (parseInt(result) === 1) {
                    toastr.success("Partner have been created successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else if (parseInt(result) === 2) {
                    toastr.success("Partner have been updated successfully !", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while saving Partner, please try again later", "Error", { timeOut: 3000 });
                }

            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving Partner, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}
