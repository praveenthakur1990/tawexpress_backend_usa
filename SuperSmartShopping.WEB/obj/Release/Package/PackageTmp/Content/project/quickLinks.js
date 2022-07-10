$(document).ready(function () {
    // Add minus icon for collapse element which is open by default
    $(".collapse.show").each(function () {
        $(this).prev(".card-header").find(".fa").addClass("fa-minus").removeClass("fa-plus");
    });

    // Toggle plus minus icon on show hide of collapse element
    $(".collapse").on('show.bs.collapse', function () {
        $(this).prev(".card-header").find(".fa").removeClass("fa-plus").addClass("fa-minus");
    }).on('hide.bs.collapse', function () {
        $(this).prev(".card-header").find(".fa").removeClass("fa-minus").addClass("fa-plus");
    });
    $('#frmAddUpdateQuickPages').validate({});
    $('.summernote').summernote();
})

function saveQuickPages() {
    $("form#frmAddUpdateQuickPages input.validate").each(function () {
        $(this).rules("add",
            {
                required: false,
                maxlength: 200,
                url: true
            })

    });
    if ($('#frmAddUpdateQuickPages').valid()) {
        var quickLinks = [];
        $("div.quickLink").each(function () {
            var id = $(this).attr('data-content');
            var data = {
                'Id': id,
                'Name': $.trim($(this).find('label.pageName').text()),
                'Link': $(this).find('input[name="name_' + id + '"]').val(),
                'PageContent': $(this).find('.note-editable').text() != '' ? $(this).find('.note-editable').html() : '',
                'IsActive': $(this).find('input[type=checkbox]').prop('checked'),
                'CreatedBy': $("#hdnUserId").val()
            }
            quickLinks.push(data);
        })

        console.log(quickLinks)
        $.ajax({
            url: "/Admin/Setting/AddUpdateQuickPages",
            method: "POST",
            data: JSON.stringify(quickLinks),
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
                    toastr.success("Quick links has been updated successfully !", "Success", { timeOut: 3000 });
                    //refreshPageCustom(location, 1000);
                }
                else {
                    toastr.error("An error occured while updaing Quick links, please try again later", "Error", { timeOut: 3000 });
                }
            },
            error: function (jqXHR) {
                ajaxindicatorstop();
                toastr.error("An error occured while updaing Quick links, please try again later", "Error", { timeOut: 3000 });
            }
        });
    }
}

