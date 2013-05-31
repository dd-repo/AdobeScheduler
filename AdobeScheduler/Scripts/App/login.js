/// <reference path="~/Scripts/jquery-1.9.1.js" />
/// <reference path="~/Scripts/jquery.signalR-1.0.0.js" />
/// <reference path="~/Scripts/fullcalendar.js" />

$(function () {
    var adobeConnect = $.connection.adobeConnect;

    $.connection.hub.start().done(function () {
        $('button#login').click(function (e) {
            adobeConnect.server.login($('#uname').val(), $('#pass').val()).done(function (res) {
                if (res != "") {
                    $('#request').html("<iframe src='" + res + "'" + " ></iframe>");
                    setTimeout(function () {
                        $('#loginform').submit();
                    }, 100);

                } else {
                    html = "<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Error!</strong> Invalid Credentials </div>";
                    $('#error').html(html);
                }

            })
        });
    });
});
