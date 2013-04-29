/// <reference path="~/Scripts/jquery-1.9.1.js"" />
/// <reference path="~/Scripts/jquery.signalR-1.0.0.js" />
/// <reference path="~/Scripts/fullcalendar.js" />

$(function () {

    //DatPicker Set Up 
    $('#date').datepicker({
        
    });

    // TimePicker Set Up
    $('#time').timepicker();

    var adobeConnect = $.connection.adobeConnect;

    adobeConnect.client.addSelf = function (event) {
        var array = $('#calendar').fullCalendar('clientEvents');
        for (i in array) {
            if (event.end >= array[i].start && event.start <= array[i].end) {
                alert('overlapping');
            }
        }
        $('#calendar').fullCalendar('renderEvent', event, true);
        console.log(event);
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

    adobeConnect.client.addEvent = function (s) {
        adobeConnect.server.addSelf(s,$('#content').attr('data-userId'))
    }

    $.connection.hub.start().done(function () {
        adobeConnect.server.getAllAppointments()
            .done(function (data) {
                alert("done");
                console.log(data);
                $('.spinner').remove();
                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,agendaWeek,agendaDay'
                    },
                    defaultView: 'agendaWeek',
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

       
        $('#AppointMent_Submit').click(function () {
            var class_name = $('#class option:selected').text();
            var url = $('#class option:selected').attr('data-url');
            var path = $('#class option:selected').attr('data-path');
            var userId = $('#content').attr('data-userId');
            var date = $('#date').val();
            var time = $('#time').val();
            var room_size = $('#room_size').val();
            var end = $('#duration option:selected').text();
            if (room_size != "") {
                adobeConnect.server.addAppointment(userId, class_name, room_size, url, path, date, time,end);
                $('#addAppointment').modal('hide');
            }
        });

        $('#loginform').submit(function (e) {
            
            adobeConnect.server.login($('#uname').val(), $('#pass').val()).done(function (res) {
                e.preventDefault;
            });
        });
    });


});
