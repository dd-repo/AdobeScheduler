/// <reference path="~/Scripts/jquery-1.9.1.js"" />
/// <reference path="~/Scripts/jquery.signalR-1.0.0.js" />
/// <reference path="~/Scripts/fullcalendar.js" />

$(function () {
    window.isUpdate = false;
    var adobeConnect = $.connection.adobeConnect;


    addAppointment = function (checked, isUpdate) {
        var Id;
        if (window.Id == 'undefined') {
            Id = $('#content').attr('data-userId');
        }
        else { Id = window.Id; }
        var class_name = $('#class option:selected').text();
        var url = $('#class option:selected').attr('data-url');
        var path = $('#class option:selected').attr('data-path');
        var date = $('#date').val();
        var time = $('#time').val();
        var room_size = $('#occupants').val();
        var end = $('#duration option:selected').text();
        adobeConnect.server.addAppointment(checked,isUpdate, Id, class_name, room_size, url, path, date, time, end);
        if (checked) { $('#addAppointment').modal('hide'); }
    }

    //DatPicker Set Up 
    $('#date').datepicker({}).on('changeDate', function () {
        if ($('#AppointMent_Submit').val() == "Create Appointment" && $('#duration option:selected').text() != '') {
            addAppointment(false);
        }
    });
    // TimePicker Set Up
    $('#time').timepicker();

   

    adobeConnect.client.addSelf = function (add, event, max) {
        console.log(event.roomSize,event.start, max);
       var html = "<div class='alert alert-info'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Warning!</strong> A maximum of <b> " + max + "</b> occupants <small> <u>including the host</u> </small> are avaible" + "</div>"
        $("#AppointMent_Submit").prop("disabled", true);
        if (event.roomSize > max) {
            html = "<div class='alert alert-warning'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Warning!</strong> Seats are filled or you are over the alloted maximum of <b>" +max+ "</b></div>";
        }
        $('#error').html(html);
        if (event.roomSize <= max && add) {
            $('#calendar').fullCalendar('renderEvent', event, true);
        }

        if (event.roomSize <= max) {
            $('#AppointMent_Submit').removeAttr('disabled');
        }
        
    }

    adobeConnect.client.removeSelf = function (id) {
        $('#calendar').fullCalendar( 'removeEvents' ,id)
    }

    getDate = function (date) {
        date = new Date(date);
        return date.toLocaleDateString();
    }

    getDuration = function (start, end) {
        return(parseInt(end-start)/(1000*60));
    }

    convertTime = function (date) {
        var oclock = 'AM';
        var minutes = date.getMinutes();
        if (date.getHours() == 0) {
            hours = "12";
        }
        if (date.getHours() < 10) {
            var hours = "0" + date.getHours();
        } else {
            hours = date.getHours();
        }
                
        if (date.getHours() >= 12) {
            oclock = 'PM';
            if (date.getHours() == 12) {
                hours = date.getHours();
            } else if (date.getHours() < 22)  {
                hours = "0" + (date.getHours() - 12);
            } else {
                hours = date.getHours() - 12;
            }
        }
        if (date.getHours() == 0) {
            hours = "12";
        }
        
        if (date.getMinutes() == 0) {
            minutes = "00";
        }

        return hours + ":" + minutes + " " + oclock;

    }


    adobeConnect.client.userinfo = function (data) {
        console.log(data);
    }

    adobeConnect.client.count = function (data) {
        console.log(data);
    }

    adobeConnect.client.addEvent = function (s, checked, isUpdate) {
        adobeConnect.server.addSelf(s,$('#content').attr('data-userId'),checked,isUpdate)
    }
    
    

    $('input#AppointMent_Submit').on('click', function () {
        alert(1);
    });

    $.connection.hub.start().done(function () {
        adobeConnect.server.getAllAppointments()
            .done(function (data) {
                $('.spinner').remove();
                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today month',
                        center: 'title',
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
                            var html = '<a id="editEvent" href="#'+event.id+'"><i class="icon-info-sign" style="float:right;"></i></a>'
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
                        if (view.name == 'month') {
                            $('#calendar').fullCalendar('changeView', 'agendaDay');
                            $('#calendar').fullCalendar('gotoDate', date);

                        }

                        if (view.name == 'agendaDay') {
                            $('#date').val(date.toLocaleDateString());
                            $('#time').val(convertTime(date));
                            var html = "<input disabled type='submit' id='AppointMent_Submit' onclick='addAppointment(true,false)' class='btn btn-success' value='Create Appointment' />";
                            $('.modal-footer').html(html);
                            $('#addAppointment').modal('show');
                        }
                    },
                    eventClick: function (event, element) {
                        if (element.target.className == 'icon-info-sign') {
                            window.Id = event.id;
                            window.isUpdate = true;
                            var cal_hash = element.target.parentElement.hash;
                            $('#class option:selected').text(event.title);
                            $('#date').val(getDate(event.start));
                            $('#time').val(convertTime(event.start));
                            $('#occupants').val(event.roomSize);
                            $('#duration option:selected').text(getDuration(event.start, event.end));
                            var html = "<input type='submit' onclick='delete_confirm()' class='btn btn-danger' style='float:left' value='Delete Appointment' /><input disabled type='submit' onclick='Update()' id='AppointMent_Submit' class='btn btn-success' value='Update Appointment' />";
                            $('.modal-footer').html(html);
                            $('#addAppointment').modal('show');

                            delete_confirm = function () {
                                var r = confirm("Are you sure you want to permanantly delete this appoinment?");
                                if (r==true)
                                {
                                    adobeConnect.server.delete(event.id);
                                    $('#addAppointment').modal('hide');
                                }
                            }

                            Update = function () {
                                addAppointment(true, true);
                            }
                        }
                        else {
                            if (event.url) window.open(event.url, event.title);
                        }
                        return false;
                    }

                });
            });

       
        $('button#login').click(function (e) {
            adobeConnect.server.login($('#uname').val(), $('#pass').val()).done(function (res) {
                if (res != "") {
                    $('#request').html("<iframe src='" + res + "'" + " ></iframe>");
                    setTimeout(function () {
                        $('#loginform').submit();
                    },100);
                   
                } else {
                    html = "<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button><strong style='float:left;'>Error!</strong> Invalid Credentials </div>";
                    $('#error').html(html);
                }
                
            });
        });
        
        $('.numbersOnly').keyup(function () {
            this.value = this.value.replace(/[^0-9\.]/g, '');
        });

        $('#occupants').keyup(function (e) {
            if ($('#duration option:selected').text() != '') {
                addAppointment(false,window.isUpdate);
            }
        });


        $('#time').on('change', function () {
            if ($('#duration option:selected').text() != '') {
                addAppointment(false,window.isUpdate);
            }
        });

        $('#class').blur(function (e) {
            if ($('#duration option:selected').text() != '') {
                addAppointment(false,window.isUpdate);
            }
        });

        $('#addAppointment').on('show', function () {
            if($('#duration option:selected').text() != ''){
                addAppointment(false,window.isUpdate);
            }
        });
        

        $('#addAppointment').on('hide', function () {
          $('#occupants').val('50');

        });
        $('#reserve_room').click(function () {
            var html = "<input disabled type='submit' id='AppointMent_Submit' onclick='addAppointment(true)' class='btn btn-success' value='Create Appointment' />";
            $('.modal-footer').html(html);
            if ($('#date').val() == "") {
                date = new Date();
                $('#date').val(date.toLocaleDateString());
            }
        });
        

        
    });


});
