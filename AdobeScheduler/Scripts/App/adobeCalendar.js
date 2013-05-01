/// <reference path="~/Scripts/jquery-1.9.1.js"" />
/// <reference path="~/Scripts/jquery.signalR-1.0.0.js" />
/// <reference path="~/Scripts/fullcalendar.js" />

$(function () {

    //DatPicker Set Up 
    $('#date').datepicker({}).on('changeDate', function (e) {
        console.log(e);
        var class_name = $('#class option:selected').text();
        var url = $('#class option:selected').attr('data-url');
        var path = $('#class option:selected').attr('data-path');
        var userId = $('#content').attr('data-userId');
        var date = $('#date').val();
        var time = $('#time').val();
        var room_size = $('#occupants option:selected').text();
        var end = $('#duration option:selected').text();
        adobeConnect.server.addAppointment(false, userId, class_name, room_size, url, path, date, time, end);
    });

    // TimePicker Set Up
    $('#time').timepicker();

    var adobeConnect = $.connection.adobeConnect;

    adobeConnect.client.addSelf = function (add, event, max) {
        $("#AppointMent_Submit").prop("disabled", true);
        html ="<div class='alert alert-warning'>A maximum of "+max+" are avaible"+"</div>"
        $('#error').html(html);
        if (event.roomSize <= max && add) {
            $('#calendar').fullCalendar('renderEvent', event, true);
        }

        if (event.roomSize <= max) {
            $('#AppointMent_Submit').removeAttr('disabled');
        }
        
    }


    adobeConnect.client.responceMessage = function (message) {
        alert(message);
    }

    adobeConnect.client.userinfo = function (data) {
        console.log(data);
    }

    adobeConnect.client.count = function (data) {
        console.log(data);
    }

    adobeConnect.client.addEvent = function (s,checked) {
        adobeConnect.server.addSelf(s,$('#content').attr('data-userId'),checked)
    }

    $.connection.hub.start().done(function () {
        adobeConnect.server.getAllAppointments()
            .done(function (data) {
                console.log(data);
                $('.spinner').remove();
                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,agendaWeek,agendaDay'
                    },
                    defaultView: 'month',
                    editable: false,
                    eventRender: function (event, element, view) {
                        element.find(".fc-event-inner")
                                 .append("&nbsp;&nbsp;<b>Room-Size</b>:" + event.roomSize);
                        if ($('#content').attr('data-userId') == event.userId) {
                            var html = '<a href="#"><i class="icon-info-sign" style="float:right;"></i></a>'
                            element.find(".fc-event-time").append(
                                    (html));
                            
                        }

                        else {
                            
                        }
                      
                    },
                    events: data,

                });
            });

       
        $('#occupants').blur(function (e) {
            var class_name = $('#class option:selected').text();
            var url = $('#class option:selected').attr('data-url');
            var path = $('#class option:selected').attr('data-path');
            var userId = $('#content').attr('data-userId');
            var date = $('#date').val();
            var time = $('#time').val();
            var room_size = $('#occupants option:selected').text();
            var end = $('#duration option:selected').text();
            adobeConnect.server.addAppointment(false, userId, class_name, room_size, url, path, date, time, end);
        });


        $('#time').timepicker().on('changeTime.timepicker', function (e) {
            var class_name = $('#class option:selected').text();
            var url = $('#class option:selected').attr('data-url');
            var path = $('#class option:selected').attr('data-path');
            var userId = $('#content').attr('data-userId');
            var date = $('#date').val();
            var time = $('#time').val();
            var room_size = $('#occupants option:selected').text();
            var end = $('#duration option:selected').text();
            adobeConnect.server.addAppointment(false, userId, class_name, room_size, url, path, date, time, end);
        });

        $('#class').blur(function (e) {
            var class_name = $('#class option:selected').text();
            var url = $('#class option:selected').attr('data-url');
            var path = $('#class option:selected').attr('data-path');
            var userId = $('#content').attr('data-userId');
            var date = $('#date').val();
            var time = $('#time').val();
            var room_size = $('#occupants option:selected').text();
            var end = $('#duration option:selected').text();
            adobeConnect.server.addAppointment(false, userId, class_name, room_size, url, path, date, time, end);
        });


        $('#AppointMent_Submit').click(function () {
            var class_name = $('#class option:selected').text();
            var url = $('#class option:selected').attr('data-url');
            var path = $('#class option:selected').attr('data-path');
            var userId = $('#content').attr('data-userId');
            var date = $('#date').val();
            var time = $('#time').val();
            var room_size = $('#occupants option:selected').text();
            var end = $('#duration option:selected').text();
            adobeConnect.server.addAppointment(true, userId, class_name, room_size, url, path, date, time, end);
            $('#addAppointment').modal('hide');
        });

        
        $('#loginform').submit(function (e) {
            
            adobeConnect.server.login($('#uname').val(), $('#pass').val()).done(function (res) {
                e.preventDefault;
            });
        });
    });


});
