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

    adobeConnect.client.addEvent = function (data) {
        $('#calendar').fullCalendar('renderEvent', data);
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
                            element.find('.fc-event-inner').removeClass('fc-event-skin');
                            element.find('.fc-event-head').removeClass('fc-event-skin');
                            element.find('.fc-event-head').addClass('fc-event-skin-red');
                            element.find('.fc-event-inner').addClass('fc-event-skin-red');
                            var html = '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="#"><i class="icon-info-sign"></i></a>'
                            element.find(".fc-event-content")
                                    .append(html);
                            
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
            if (room_size != "") {
                adobeConnect.server.addAppointment(userId, class_name, room_size, url, path, date, time);
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
