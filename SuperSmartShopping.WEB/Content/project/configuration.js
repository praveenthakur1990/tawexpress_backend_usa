var radiusKmInMet = 1609.34;
$(document).ready(function () {
    init()
    $('#frmAddUpdateAPIKey').validate({ // initialize the plugin
        rules: {
            publishablekey: {
                required: true
            },
            secretKey: {
                required: true
            }
        }
    });

    $('#frmAddUpdateCharges').validate({ // initialize the plugin
        rules: {
            deliveryCharges: {
                required: true
            },
            tax: {
                required: true
            }
        }
    });

    $('#frmDeliverAreaSetting').validate({ // initialize the plugin
        rules: {
            minOrderedAmt: {
                required: true
            },
            deliveryAreaInMiles: {
                required: true
            }
        }
    });

});

function addUpdateAPIKey() {
    if ($("#frmAddUpdateAPIKey").valid()) {
        $.ajax({
            url: "/Admin/Setting/AddUpdateStripeKey",
            method: "POST",
            data: { 'publishablekey': $("#publishablekey").val(), 'secretkey': $("#secretKey").val() },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                if (parseInt(result) === 1) {
                    toastr.success("API Key has been updated successfully !", "Success", { timeOut: 3000 });
                }
                else {
                    toastr.error("An error occured while saving API Key, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while saving API Key, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function init() {
    var mapCenter = new google.maps.LatLng($("#hdnLatitude").val(), $("#hdnLongitude").val());
    var map = new google.maps.Map(document.getElementById('map'), {
        'zoom': 11,
        'center': mapCenter,
        'mapTypeId': google.maps.MapTypeId.ROADMAP
    });

    // Create a draggable marker which will later on be binded to a
    // Circle overlay.
    var marker = new google.maps.Marker({
        map: map,
        position: new google.maps.LatLng($("#hdnLatitude").val(), $("#hdnLongitude").val()),
        draggable: true,
        title: 'Drag me!'
    });

    // Add a Circle overlay to the map.
    var circle = new google.maps.Circle({
        map: map,
        radius: parseFloat($("#deliveryAreaInMiles").val() > 0 ? parseFloat($("#deliveryAreaInMiles").val() * 1609.34) : radiusKmInMet)
    });

    circle.bindTo('center', marker, 'position');

    google.maps.event.addDomListener(window, 'load', init);

    $("#myslide").slider({
        orientation: "horizontal",
        range: "min",
        max: 50,
        min: 1,
        value: parseFloat($("#deliveryAreaInMiles").val() > 0 ? parseFloat($("#deliveryAreaInMiles").val()) : 1),
        slide: function (event, ui) {
            updateRadius(circle, ui.value);
        }
    });

    function updateRadius(circle, rad) {
        var km = parseFloat(rad);
        var mi = "";
        if (!isNaN(km)) mi = km * 0.621371192;
        var selectedradius = rad * 1000;
        console.log(selectedradius)
        $("input#deliveryAreaInMiles").val(parseFloat(mi).toFixed(2));
        circle.setRadius(selectedradius);
    }
}

function addUpdateCharges() {

    if ($("#frmAddUpdateCharges").valid()) {
        $.ajax({
            url: "/Admin/Setting/AddUpdateDeliveryChargesTaxes",
            method: "POST",
            data: {
                'tax': $("#tax").val() == '' ? 0 : $("#tax").val(), 'charges': $("#deliveryCharges").val() == '' ? 0 : $("#deliveryCharges").val(), 'isCashOnDelivery': $("#isCashOnDelivery").is(":checked")
            },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                console.log(result)
                if (parseInt(result) === 1) {
                    toastr.success("Charges have been updated successfully !", "Success", { timeOut: 3000 });
                }
                else {
                    toastr.error("An error occured while updating the charges, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updating the charges, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function saveDeliveryAreaSetting() {
    if ($("#frmDeliverAreaSetting").valid()) {
        $.ajax({
            url: "/Admin/Setting/AddUpdateDeliveryAreaSetting",
            method: "POST",
            data: {
                'minOrderAmt': $("#minOrderedAmt").val(), 'maxDeliveryAreaInMiles': $("#deliveryAreaInMiles").val()
            },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                if (parseInt(result) === 1) {
                    toastr.success("Delivery area setting has been updated successfully !", "Success", { timeOut: 3000 });
                }
                else {
                    toastr.error("An error occured while updating the setting, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updating the setting, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}
