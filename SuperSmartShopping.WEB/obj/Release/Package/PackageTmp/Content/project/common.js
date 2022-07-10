
$(document).ready(function () {
    var date = new Date();
    var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());

    $('.dataTables-example').DataTable({
        dom: '<"html5buttons"B>lTfgitp',
        "paging": true,
        "ordering": true,
        "info": false,
        language: { search: '', searchPlaceholder: "Search..." },
        buttons: [
            //{ extend: 'copy' },
            //{ extend: 'csv' },
            //{ extend: 'excel', title: 'ExampleFile' },
            //{ extend: 'pdf', title: 'ExampleFile' },

            //{
            //    extend: 'print',
            //    customize: function (win) {
            //        $(win.document.body).addClass('white-bg');
            //        $(win.document.body).css('font-size', '10px');

            //        $(win.document.body).find('table')
            //            .addClass('compact')
            //            .css('font-size', 'inherit');
            //    }
            //}
        ]
    });

    $("input.goOnlineOffline").bootstrapSwitch({
        onText: 'Online',
        offText: 'Offline',
        onColor: 'success',
        offColor: 'danger',
        size: 'medium'
    });

    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
    });

    $('.input-group.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: false,
        autoclose: true,
        //startDate: today       
    });

    $('.input-group.weeklycirculardate').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: false,
        autoclose: true,
        format: 'dd/mm/yyyy'
    });
    // $('.input-group.date').datepicker('setDate', today);

    //$(".noSpecialChar").keypress(function (e) {
    //    var key = e.keyCode || e.which;
    //    //Regular Expression
    //    var reg_exp = /^[A-Za-z0-9 ]+$/;
    //    //Validate Text Field value against the Regex.
    //    var is_valid = reg_exp.test(String.fromCharCode(key));
    //    if (!is_valid) {
    //        toastr.error("No special characters Please", "", { timeOut: 500 })
    //        //$("#error_msg").html("No special characters Please!");
    //    }
    //    return is_valid;
    //});

    $('input.goOnlineOffline').on('switchChange.bootstrapSwitch', function (e, data) {
        $("span.onlineOfflinetext").text(data == true ? "Online" : "Offline");
        $('div#goOnlineOfflineModal').find("#onlineOfflineStatus").val(data);
        $('input.goOnlineOffline').bootstrapSwitch('state', !data, true);
        $('div#goOnlineOfflineModal').modal({
            backdrop: 'static',
            keyboard: false
        });
    });
    $(".modal-footer .btn-primary").click(function () {
        $('input.goOnlineOffline').bootstrapSwitch('toggleState', true, true);
        $('div#goOnlineOfflineModal').modal('hide')
    })
});

function logout() {
    $.ajax({
        type: 'POST',
        url: '/Account/Logout',
    }).done(function (data) {
        toastr.info("Thank you, goodbye !", { timeOut: 3000 })
        window.location.href = '/Account/Login';
    }).fail(function () {
        toastr.error("An error occured while logout, please try again later", "", { timeOut: 1000 });
    });
}

function returnLoadingText() {
    return "Please wait...";
}

function ajaxindicatorstart(text) {
    if (jQuery('body').find('#resultLoading').attr('id') != 'resultLoading') {
        jQuery('body').append('<div id="resultLoading"><div><img src="/Content/img/rotating-balls-spinner.svg"><div>' + text + '</div></div><div class="bg"></div></div>');
    }
    jQuery('#resultLoading').css({
        'width': '100%',
        'height': '100%',
        'position': 'fixed',
        'z-index': '10000000',
        'top': '0',
        'left': '0',
        'right': '0',
        'bottom': '0',
        'margin': 'auto'
    });

    jQuery('#resultLoading .bg').css({
        'background': '#000000',
        'opacity': '0.7',
        'width': '100%',
        'height': '100%',
        'position': 'absolute',
        'top': '0'
    });

    jQuery('#resultLoading>div:first').css({
        'width': '250px',
        'height': '75px',
        'text-align': 'center',
        'position': 'fixed',
        'top': '0',
        'left': '0',
        'right': '0',
        'bottom': '0',
        'margin': 'auto',
        'font-size': '16px',
        'z-index': '10',
        'color': '#ffffff'

    });

    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeIn(300);
    jQuery('body').css('cursor', 'wait');
}

function ajaxindicatorstop() {
    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeOut(300);
    jQuery('body').css('cursor', 'default');
}

function refreshPageCustom(location, duration) {
    return setTimeout(function () {
        window.location.href = location;
    }, duration);
}

$('.number-format').usPhoneFormat({
    format: '(xxx) xxx-xxxx',
});

$('body').delegate('.numbers-only', 'keypress', function (e) {
    if (e.which == 46) {
        if ($(this).val().indexOf('.') != -1) {
            return false;
        }
    }
    if (e.which != 8 && e.which != 0 && e.which != 46 && (e.which < 48 || e.which > 57)) {
        return false;
    }
})

function updateOnlineOffline() {
    var currentState = $("#onlineOfflineStatus").val();
    $.ajax({
        url: "/Admin/Dashboard/SetIsOnlineOffline",
        method: "POST",
        data: { 'IsOnline': $("#onlineOfflineStatus").val() },
        beforeSend: function () {
            ajaxindicatorstart(returnLoadingText());
        },
        success: function (result) {
            ajaxindicatorstop();
            $('div#goOnlineOfflineModal').modal('hide')
            var location = window.location.href;
            if (parseInt(result) === 1) {
                //toastr.success("Status have been updated successfully !", "Success", { timeOut: 1000 });
                refreshPageCustom(location, 0);
            }
            else {
                toastr.error("An error occured while updating status, please try again later", "Error", { timeOut: 1000 });
            }
        },
        error: function (jqXHR) {
            ajaxindicatorstop();
            toastr.error("An error occured while updating status, please try again later", "Error", { timeOut: 1000 });
        }
    });
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}