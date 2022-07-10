var selectedOrder = [], selectedRider = [];
$('select.riderDDL').selectpicker();
function btnGetSearchOrder() {
    window.location.href = '/admin/Order/Index?page=1&searchStr=' + $("#txtSearch").val() + '';
}

function trackOrder(orderId) {
    $.ajax({
        url: "/Admin/Orders/TrackOrder",
        method: "POST",
        data: { 'OrderId': orderId },
        beforeSend: function () {
            ajaxindicatorstart(returnLoadingText());
        },
        success: function (result) {
            ajaxindicatorstop();
            $("div#trackOrderModal").find('div.modal-body').html('');
            $("div#trackOrderModal").find('div.modal-body').html(result);
            $("div#trackOrderModal").modal('show');
        },
        error: function (jqXHR) {
            ajaxindicatorstop();
            toastr.error("An error occured while reteriving order track status, please try again later", "Error", { timeOut: 3000 });
        }
    });
}

function viewOrder(orderId) {
    $.ajax({
        url: "/Admin/Order/ViewOrderDetails",
        method: "POST",
        data: { 'OrderId': orderId },
        beforeSend: function () {
            ajaxindicatorstart(returnLoadingText());
        },
        success: function (result) {
            ajaxindicatorstop();
            $("div#viewOrderModal").find('div.modal-body').html('');
            $("div#viewOrderModal").find('div.modal-body').html(result);
            $("div#viewOrderModal").modal('show');
            $(document).ready(function () {
                $('#frmOrderStatus').validate({ // initialize the plugin
                    rules: {
                        status: {
                            required: true
                        }
                    }
                });
            });
        },
        error: function (jqXHR) {
            ajaxindicatorstop();
            toastr.error("An error occured while reteriving order details, please try again later", "Error", { timeOut: 3000 });
        }
    });
}

function updateStatus() {
    if ($("#frmOrderStatus").valid()) {
        $.ajax({
            url: "/Admin/Order/UpdateStatus",
            method: "POST",
            data: $("#frmOrderStatus").serialize(),
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                if (result.res === 1) {
                    toastr.success("Order Status have been updated successfully", "Success", { timeOut: 3000 });
                    window.location.reload();
                }
                else {
                    toastr.error("An error occured while updating order status, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updating order status, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

function print(orderId) {
    for (var i = 0; i < 2; i++) {
        $.ajax({
            url: "/Admin/Order/ViewPrintOrder",
            method: "POST",
            data: { 'OrderId': orderId, 'IsCustomerCopy': (i == 0 ? false : true) },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var disp_setting = "toolbar=yes,location=no,";
                disp_setting += "directories=yes,menubar=yes,";
                disp_setting += "scrollbars=yes,width=650, height=600, left=100, top=25";
                var docprint = window.open("", "", disp_setting);
                docprint.document.open();
                docprint.document.write('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN"');
                docprint.document.write('"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">');
                docprint.document.write('<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">');
                docprint.document.write('<head><title>My Title</title>');
                docprint.document.write('<style type="text/css">body{ margin:0px;');
                docprint.document.write('font-family:verdana,Arial;color:#000;');
                docprint.document.write('font-family:Verdana, Geneva, sans-serif; font-size:12px;}');
                docprint.document.write('a{color:#000;text-decoration:none;} </style>');
                docprint.document.write('</head><body onLoad="self.print()"><center>');
                docprint.document.write(result.htmlStr);
                docprint.document.write('</center></body></html>');
                docprint.document.close();

                $(function () {
                    window.addEventListener("afterprint", function (event) { alert("fdsdf") });
                    window.onafterprint = function (event) { alert("fdsdf sefgs") };
                })
                // docprint.close();
                //docprint.focus();                 
                // window.close();
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while printing order, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
    //$.ajax({
    //    url: "/Admin/Orders/Print",
    //    method: "GET",
    //    data: { 'id': orderId },        
    //    success: function (result) {            
    //    },
    //    error: function (jqXHR) {
    //        ajaxindicatorstop();
    //        toastr.error("An error occured while printing order, please try again later", "Error", { timeOut: 3000 });
    //    }
    //});
}


(function () {

    var beforePrint = function () {
        alert('Functionality to run before printing.');
    };

    var afterPrint = function () {
        alert('Functionality to run after printing');
    };

    if (window.matchMedia) {
        var mediaQueryList = window.matchMedia('print');

        mediaQueryList.addListener(function (mql) {
            //alert($(mediaQueryList).html());
            if (mql.matches) {
                beforePrint();
            } else {
                afterPrint();
            }
        });
    }

    window.onbeforeprint = beforePrint;
    window.onafterprint = afterPrint;

}());

function assignOrderToRider() {
    selectedOrder = [];
    $('table.orderMargin tbody tr').find('td:first input[type="checkbox"]:checked').not(":disabled").each(function () {
        selectedOrder.push($(this).val());
    })
    if (selectedRider.length == 0) {
        toastr.error("Please select a rider", "Error", { timeOut: 1000 })
    }
    if (selectedOrder.length == 0) {
        toastr.error("Please select an order", "Error", { timeOut: 1000 })
    }
    debugger
    if (selectedRider.length > 0 && selectedOrder.length > 0) {
        $.ajax({
            url: "/Admin/Order/SendPushNotificationRider",
            method: "POST",
            data: { 'orderIds': selectedOrder, 'riderIds': selectedRider },
            traditional: true,
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (result) {
                ajaxindicatorstop();
                var location = '/Admin/Order/Index';
                if (result == 1) {
                    toastr.success("Your request has been sent successfully", "Success", { timeOut: 3000 });
                    refreshPageCustom(location, 1000);
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while sending request, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

$('select.riderDDL').change(function () {
    selectedRider = $(this).val();
});