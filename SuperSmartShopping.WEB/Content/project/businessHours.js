$(document).ready(function () {
    $('.timeInput').datetimepicker({
        datepicker: false,
        formatTime: "h:i A",
        step: 15,
        format: "h:i A",

    });

    $('#frmAddUpdateBusinessHours').validate();
    $('#frmAddMultiplePickupAddress').validate();

    $('#frmAddCustomUrl').validate({
        rules: {
            LabelText: {
                required: true,
                maxlength: 50
            },
            UrlText: {
                required: true,
                maxlength: 200
            }
        }
    });

    $(function () {
        var options = {
            //types: ['(cities)'],
            country: ["US", "IN", "ES"]
        }
    });
    var places = new google.maps.places.Autocomplete(document.getElementById('address'), options);
    google.maps.event.addListener(places, 'place_changed', function () {
        var result = places.getPlace();
    });
})


function saveBusinessHours() {
    $("form#frmAddUpdateBusinessHours input.validate").each(function () {
        $(this).rules("add",
            {
                required: true,
            })
    });
    if ($('#frmAddUpdateBusinessHours').valid()) {
        var weekDays = [];
        $("div.weekdays").each(function () {
            var dayId = $(this).attr('data-content');
            var openTime = $(this).find('input[name="openTime_' + dayId + '"]').val();
            var closeTime = $(this).find('input[name="closeTime_' + dayId + '"]').val();
            var IsClosed = $(this).find('input[type=checkbox]').prop('checked');
            var data = {
                'WeekDayId': parseInt(dayId),
                'OpenTime': openTime,
                'CloseTime': closeTime,
                'IsClosed': IsClosed
            }
            weekDays.push(data);
        })
        $.ajax({
            url: "/Admin/Setting/AddUpdateBusinessHours",
            method: "POST",
            data: JSON.stringify(weekDays),
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            traditional: true,
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                //var location = '/Admin/Setting/AddUpdateBusinessHours';
                if (parseInt(result) === 1) {
                    toastr.success("Business hours have been updated successfully !", "Success", { timeOut: 3000 });
                    //refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while updaing business hours, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updaing business hours, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function savePickupAddress() {
    $("form#frmAddMultiplePickupAddress input").each(function () {
        $(this).rules("add",
            {
                required: true,
            })
    });
    if ($('#frmAddMultiplePickupAddress').valid()) {
        var addressArr = [];
        $("div.field").each(function () {
            var address = $(this).find('input').val();
            addressArr.push(address);
        })
        console.log(addressArr.join("_ "))
        $.ajax({
            url: "/Admin/Setting/AddUpdatePickUpAddress",
            method: "POST",
            data: { 'pickupAddresses': addressArr.join("_ ") },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                if (parseInt(result) === 1) {
                    toastr.success("Pickup addresses have been updated successfully !", "Success", { timeOut: 3000 });
                }
                else {
                    toastr.error("An error occured while updaing Pickup addresses, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updaing Pickup addresses, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function markAsClosed(e, ele) {
    if ($(ele).is(':checked')) {
        $(ele).closest("div#div_" + e).find("input.validate").attr('disabled', true);
    }
    else {
        $(ele).closest("div#div_" + e).find("input.validate").attr('disabled', false);
    }
}
$('.extra-fields-customer').click(function () {
    var count = 0;
    $('.customer_records').clone().appendTo('.customer_records_dynamic');
    $('.customer_records_dynamic .customer_records').addClass('single remove');
    $('.single a.extra-fields-customer').remove();
    $('.single div.col-lg-3').append('<a href="javascript:void(0);" class="remove-field btn-remove-customer addOptionSetBtn">(-) Remove</a>');
    $('.customer_records_dynamic > .single').attr("class", "remove field");
    $('.customer_records_dynamic input').each(function () {
        var fieldname = $(this).attr("name");
        $(this).attr('name', fieldname + count);
        $(this).attr('id', fieldname + count);

        $(function () {
            var options = {
                //types: ['(cities)'],
                componentRestrictions: { country: ["US", "IN", "ES"] }
            };
            var places = new google.maps.places.Autocomplete(document.getElementById(fieldname + count), options);
            google.maps.event.addListener(places, 'place_changed', function () {
                var result = places.getPlace();
            });
        })
        count++;
    });

});

$(document).on('click', '.remove-field', function (e) {
    $(this).closest("div.remove").remove();
    count--;
    e.preventDefault();
});

function saveCustomUrl() {
    if ($('#frmAddCustomUrl').valid()) {
        var obj = {
            'LabelText': $("#LabelText").val(),
            'UrlText': $("#UrlText").val(),
            'IsActive': $('input[name=IsActive]').prop('checked')
        }
        console.log(obj)
        $.ajax({
            url: "/Admin/Setting/AddCustomUrl",
            method: "POST",
            data: obj,
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                if (parseInt(result) === 1) {
                    toastr.success("Pickup addresses have been updated successfully !", "Success", { timeOut: 3000 });
                }
                else {
                    toastr.error("An error occured while updaing Pickup addresses, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updaing Pickup addresses, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}