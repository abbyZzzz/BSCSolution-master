
/*
 Template Name: Stexo - Responsive Bootstrap 4 Admin Dashboard
 Author: Themesdesign
 Website: www.themesdesign.in
 File: Calendar init js
 */

!function($) {
    "use strict";

    var CalendarPage = function() {};

    CalendarPage.prototype.init = function() {

        //checking if plugin is available
        if ($.isFunction($.fn.fullCalendar)) {
            /* initialize the external events */
            $('#external-events .fc-event').each(function() {
                // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
                // it doesn't need to have a start or end
                var eventObject = {
                    title: $.trim($(this).text()) // use the element's text as the event title
                };

                // store the Event Object in the DOM element so we can get to it later
                $(this).data('eventObject', eventObject);

                // make the event draggable using jQuery UI
                $(this).draggable({
                    zIndex: 999,
                    revert: true, // will cause the event to go back to its
                    revertDuration: 0 //  original position after the drag
                });

            });
            
            /* initialize the calendar */

            var date1 = new Date();
            var d = date1.getDate();
            var m = date1.getMonth();
            var y = date1.getFullYear();

            $('#calendar').fullCalendar({
                header: {
                    left: '',
                    center: 'title',
                    right: 'prev,next today'
                },
                editable: true,
                eventLimit: true, // allow "more" link when too many events
                droppable: true, // this allows things to be dropped onto the calendar !!!
                dayRender: function (date, cell) {
                    if (date.date() === d && date.month() === m) {
                        cell.css("background-color","rgb(243, 243, 243)");
                    }
                },
                drop: function (date, allDay, jsEvent, ui) { // this function is called when something is dropped

                    // retrieve the dropped element's stored Event Object
                    var originalEventObject = $(this).data('eventObject');

                    // we need to copy it, so that multiple events don't have a reference to the same object
                    var copiedEventObject = $.extend({}, originalEventObject);

                    // assign it the date that was reported
                    //var evt = {
                    //    title: copiedEventObject.title,
                    //    start: date._d,
                    //    allDay: allDay
                    //}
                    copiedEventObject.start = date._d;
                    copiedEventObject.allDay = allDay;
                     //render the event on the calendar
                     //the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
                    $('#calendar').fullCalendar('renderEvent', copiedEventObject, true);
                    //$('#calendar').fullCalendar('renderEvent', evt, true);
                    dropEvt(copiedEventObject);

                },
                events: function (start,end,timezone,callback) {
                    
                    var schedule_id = $("#schedule_id").val();
                    $.ajax({
                        type: "POST",
                        url: "/Schedule/GetProgramScheduleTime",
                        data: {
                            schedule_id: schedule_id
                        },
                        success: function (data) {
                            var events = [];
                            $.each(data, function (key, value) {
                                events.push({
                                    title: value.schedule_day_name,
                                    start: value.schedule_date,
                                    end: value.schedule_date
                                });
                            });
                            callback(events);
                        },
                        error: function (result) {
                        }
                    })
                },
                eventClick: function (event) {
                    $("#myDelModal").modal("show");
                    $("#confirmDelete").click(function () {
                        $('#calendar').fullCalendar("removeEvents", event._id);
                        $("#myDelModal").modal("hide");
                    });
                },
                eventDrop: function (event) {
                    var evt = {
                        title: event.title,
                        start: event.start._d,
                        allDay: event.allDay
                    }
                    dropEvt(evt);
                },
                eventResize: function (event) {
                    //当拖拽后，删除所有id不一样的排程
                    var _self = event;
                    $('#calendar').fullCalendar('removeEvents', function (event) {
                        if (_self._id !== event._id && event.start._d.getTime() > _self.start._d.getTime() && event.start._d.getTime() < _self.end._d.getTime()) {
                            return true;
                        }
                    });
                }
            });
            
             /*Add new event*/
            // Form to add new event

            $("#add_event_form").on('submit', function(ev) {
                ev.preventDefault();

                var $event = $(this).find('.new-event-form'),
                    event_name = $event.val();

                if (event_name.length >= 3) {

                    var newid = "new" + "" + Math.random().toString(36).substring(7);
                    // Create Event Entry
                    $("#external-events").append(
                        '<div id="' + newid + '" class="fc-event">' + event_name + '</div>'
                    );


                    var eventObject = {
                        title: $.trim($("#" + newid).text()) // use the element's text as the event title
                    };

                    // store the Event Object in the DOM element so we can get to it later
                    $("#" + newid).data('eventObject', eventObject);

                    // Reset draggable
                    $("#" + newid).draggable({
                        revert: true,
                        revertDuration: 0,
                        zIndex: 999
                    });

                    // Reset input
                    $event.val('').focus();
                } else {
                    $event.focus();
                }
            });

        }
        else {
            alert("Calendar plugin is not installed");
        }
    },
    //init
    $.CalendarPage = new CalendarPage, $.CalendarPage.Constructor = CalendarPage
}(window.jQuery),

//initializing 
function($) {
    "use strict";
    $.CalendarPage.init()
}(window.jQuery);


