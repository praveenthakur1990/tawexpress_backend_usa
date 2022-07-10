$(document).ready(function () {
    $('#frmLogin').validate({ // initialize the plugin
        rules: {
            email: {
                required: true
            },
            password: {
                required: true,
                minlength: 6
            }
        }
    });
    $('#frmForgetpassword').validate({ // initialize the plugin
        rules: {
            forgetEmail: {
                required: true
            }
        }
    });

    $('#frmResetPassword').validate({ // initialize the plugin
        rules: {
            newPassword: {
                required: true
            },
            confirmPassword: {
                required: true,
                equalTo: "#newPassword"
            }
        }
    });

    $('#frmChangePassword').validate({ // initialize the plugin
        rules: {
            oldPassword: {
                required:true
            },
            newPassword: {
                required: true
            },
            confirmPassword: {
                required: true,
                equalTo: "#newPassword"
            }
        }
    });
    
});

$("#email").keypress(function (e) {
    if (e.which === 13) {
        login();
    }
});

$("#password").keypress(function (e) {
    if (e.which === 13) {
        login();
    }
});


function login() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmLogin")).val();
    if ($("#frmLogin").valid()) {
        $.ajax({
            url: '/Account/Login',
            method: 'POST',
            data: {
                email: $("#email").val(),
                password: $("#password").val(),
                __RequestVerificationToken: token,
            },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (response) {
                ajaxindicatorstop();
                console.log(response)
                if (response === "superadmin") {
                    toastr.info("Redirecting to Home Page !", "Success", { timeOut: 3000 })
                    window.location.href = '/SecurePanel/Dashboard/Index';
                }
                else if (response === "admin") {
                    toastr.info("Redirecting to Home Page !", "Success", { timeOut: 3000 })
                    window.location.href = '/Admin/Dashboard/Index';
                }
                else {
                    toastr.error(response, "Error", { timeOut: 1000 })
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error(jqXHR.responseJSON.error, "Error", { timeOut: 1000 })
            }
        });
    }
    else {
        return false;
    }
}

$("#forgetEmail").keypress(function (e) {
    if (e.which === 13) {
        btnForgetPassword();
    }
});

function btnForgetPassword() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmForgetpassword")).val();
    if ($("#frmForgetpassword").valid()) {
        $.ajax({
            url: '/Account/ForgetPassword',
            method: 'POST',
            data: { email: $("#forgetEmail").val(), __RequestVerificationToken: token },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (response) {
                ajaxindicatorstop();
                console.log(response)
                if (response === "1") {
                    toastr.info("A password reset link has been sent on your registered email address. Please check", "Success", { timeOut: 0 });
                    $("#forgetEmail").val('');
                }
                else if (response == "-1") {
                    toastr.error("Invalid username, Please enter a valid username", "Error", { timeOut: 0 })
                }
                else {
                    toastr.error(response, "Error", { timeOut: 0 })
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error(jqXHR.responseJSON.error, "Error", { timeOut: 1000 })
            }
        });
    }
    else {
        return false;
    }
}

function btnResetPassword() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmResetPassword")).val();
    if ($("#frmResetPassword").valid()) {
        $.ajax({
            url: '/Account/ResetPassword',
            method: 'POST',
            data: {
                userId: $("#hdnUserId").val(),
                code: $("#hdnCode").val(),
                newPassword: $("#newPassword").val(),
                __RequestVerificationToken: token
            },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (response) {
                ajaxindicatorstop();
                if (response === "1") {
                    toastr.info("Your password has been reset successfully", "Success", { timeOut: 0 });
                    setTimeout(function () {
                        window.location.href = '/Account/Login';
                    }, 3000);
                }
                else if (response == "Invalid token.") {
                    toastr.error("This link has been expired", "Error", { timeOut: 0 })
                    setTimeout(function () {
                        window.location.href = '/Account/ForgetPassword';
                    }, 3000);
                }
                else {
                    toastr.error(response, "Error", { timeOut: 0 })
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error(jqXHR.responseJSON.error, "Error", { timeOut: 0 })
            }
        });
    }
    else {
        return false;
    }
}

function btnChangePassword() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmChangePassword")).val();
    if ($("#frmChangePassword").valid()) {
        $.ajax({
            url: '/Setting/ChangePassword',
            method: 'POST',
            data: {
                OldPassword: $("#oldPassword").val(),
                NewPassword: $("#newPassword").val(),
                ConfirmPassword: $("#confirmPassword").val(),
                __RequestVerificationToken: token
            },
            beforeSend: function () {
                ajaxindicatorstart(returnLoadingText());
            },
            success: function (response) {
                ajaxindicatorstop();
                if (response === "1") {
                    toastr.info("Your password has been changed successfully", "Success", { timeOut: 3000 });
                    $("#oldPassword").val('');
                    $("#newPassword").val('');
                    $("#confirmPassword").val('');
                }
                else if (response == "Incorrect password.") {
                    toastr.error("Please enter valid old password", "Error", { timeOut: 3000 })                  
                }
                else {
                    toastr.error(response, "Error", { timeOut: 0 })
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error(jqXHR.responseJSON.error, "Error", { timeOut: 0 })
            }
        });
    }
    else {
        return false;
    }
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
