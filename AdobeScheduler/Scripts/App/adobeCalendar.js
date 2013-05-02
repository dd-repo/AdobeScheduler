/// <reference path="~/Scripts/jquery-1.9.1.js"" />
/// <reference path="~/Scripts/jquery.signalR-1.0.0.js" />
/// <reference path="~/Scripts/fullcalendar.js" />

$(function () {



    function validForm() {
        return false;
    }

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
        console.log(event.roomSize,event.start, max);
       var html = "<div class='alert alert-info'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Warning!</strong> A maximum of <b> " + max + "</b> occupants <small> <u>including the host</u> </small> are avaible" + "</div>"
        $("#AppointMent_Submit").prop("disabled", true);
        if (event.roomSize > max) {
            html = "<div class='alert alert-warning'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Warning!</strong> Seats are filled or you are over the alloted maximum" + "</div>";
        }
        $('#error').html(html);
        if (event.roomSize <= max && add) {
            $('#calendar').fullCalendar('renderEvent', event, true);
        }

        if (event.roomSize <= max) {
            $('#AppointMent_Submit').removeAttr('disabled');
        }
        
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
                        right: 'month'
                    },
                    defaultView: 'month',
                    editable: false,
                    eventRender: function (event, element, view) {
                        var roomHtml;
                        if (view.name == 'month') {
                            roomHtml = "</br><b>Occupants</b>: " + "<u>"+event.roomSize+"</u>";
                            element.find(".fc-event-inner")
                                     .append(roomHtml);
                        }

                        if ($('#content').attr('data-userId') == event.userId) {
                            var html = '<a href="#"><i class="icon-info-sign" style="float:right;"></i></a>'
                            element.find(".fc-event-time").append(
                                    (html));

                        }

                        if (view.name == 'agendaDay') {
                            roomHtml = "<div' style='padding-left:30%'><b>Occupants</b>: " + "<u>" + event.roomSize + "</u></div>";
                            element.find(".fc-event-time")
                                     .append(roomHtml);
                        }
                        
                    },
                    events: data,
                    dayClick: function (date, allDay, jsEvent, view) {
                        console.log(date.toLocaleDateString());
                        $('#date').val(date.toLocaleDateString());
                        $('#addAppointment').modal('show');
                    },

                });
            });

        function moveToDay(date) {
            var toDate = new Date(date);
            //alert(toDate);
            $('#calendar').fullCalendar('changeView', 'agendaDay');
            $('#calendar').fullCalendar('gotoDate', toDate);
        }




       
        $('button#login').click(function (e) {
            adobeConnect.server.login($('#uname').val(), $('#pass').val()).done(function (res) {
                if (res != "") {
                    $('#request').html("<iframe src='" + res + "'" + " ></iframe>");
                    setTimeout(function () {
                        $('#loginform').submit();
                        //location.reload();
                    },3000);
                   
                } else {
                    html = "<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Error!</strong> Invalid Credentials </div>";
                    $('#error').html(html);
                }
                
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

        
    });


});
