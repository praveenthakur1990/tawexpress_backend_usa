$(document).ready(function () {
    $('#frmAddUpdateSocialMedia').validate({

    });
})

function saveSocialLinks() {
    $("form#frmAddUpdateSocialMedia input.validate").each(function () {
        $(this).rules("add",
            {
                required: $(this).closest('div.socialMediaLink').find('input[type=checkbox]').prop('checked'),
                maxlength: 200,
                url: true
            })
    });
    if ($('#frmAddUpdateSocialMedia').valid()) {
        var socialMediaLink = [];
        $("div.socialMediaLink").each(function () {
            var id = $(this).attr('data-content');
            var data = {
                'Name': $(this).find('input[name="name_' + id + '"]').val(),
                'Icon': $(this).find('label').attr('data-content'),
                'Link': $(this).find('input[name="link_' + id + '"]').val(),
                'IsActive': $(this).find('input[type=checkbox]').prop('checked'),
            }
            socialMediaLink.push(data);
        })

        console.log(socialMediaLink)
        $.ajax({
            url: "/Admin/Setting/AddUpdateSocialMedia",
            method: "POST",
            data: JSON.stringify(socialMediaLink),
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
                    toastr.success("Social Media has been updated successfully !", "Success", { timeOut: 3000 });
                    //refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while updaing Social Media, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updaing Social Media, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}