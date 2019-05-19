$(document).ready(function () {
    var events = [];
    var selectedEvent = null;
    FetchEventAndRenderCalendar();
    function FetchEventAndRenderCalendar() {
        events = [];
        $.ajax({
            type: "GET",
            url: "/agendas/GetEvents",
            success: function (data) {
                $.each(data, function (i, v) {
                    events.push({
                        agendaId: v.AgendaId,
                        title: v.Cliente,
                        description: v.Description,
                        start: moment(v.Start),
                        color: v.ThemeColor,
                        TipoAgendamento: v.GetTipoAgendamento
                    });
                })

                GenerateCalender(events);
            },
            error: function (error) {
                alert('failed');
            }
        })
    }

    //Mostra dados sobre evendo criado no calendario
    function GenerateCalender(events) {
        $('#calender').fullCalendar('destroy');
        $('#calender').fullCalendar({
            contentHeight: 450,
            defaultDate: new Date(),
            timeFormat: 'HH(:mm) A',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,basicWeek,basicDay,agenda'
            },
            eventLimit: true,
            eventColor: '#378006',
            events: events,
            eventClick: function (calEvent, jsEvent, view) {
                selectedEvent = calEvent;
                $('#myModal #eventTitle').text(calEvent.title);
                var $description = $('<div/>');
                $description.append($('<p/>').html('<b>Data/Hora: </b>' + calEvent.start.format("DD-MMM-YYYY <br/>" + "HH:mm") + " horas"));
                $description.append($('<p/>').html('<b>Tipo Agendamento: </b>' + calEvent.TipoAgendamento));
                if (calEvent.description != null) {
                    $description.append($('<p/>').html('<b>Descrição: </b>' + calEvent.description));
                } else {
                    $description.append($('<p/>').html('<b>Descrição: </b>' + "---"));
                }
                $('#myModal #pDetails').empty().html($description);

                $('#myModal').modal();
            },
            selectable: true,
            select: function (start) {
                selectedEvent = {
                    agendaId: 0,
                    title: '',
                    description: '',
                    start: start,
                    color: ''
                };
                openAddForm();
                $('#calendar').fullCalendar('unselect');
            },
            editable: true,
            eventDrop: function (event) {
                var data = {
                    AgendaId: event.agendaId,
                    Cliente: event.title,
                    Start: event.start.format('DD/MM/YYYY HH:mm A'),
                    Description: event.description,
                    ThemeColor: event.color
                };
                SaveEvent(data);
            }
        })
    }

    $('#btnEdit').click(function () {
        //Open modal dialog for edit event
        openEditForm();
    })
    $('#btnDelete').click(function () {
        if (selectedEvent != null && confirm('Você tem certeza?')) {
            $.ajax({
                type: "POST",
                url: '/agendas/DeleteEvent',
                data: { 'agendaId': selectedEvent.agendaId },
                success: function (data) {
                    if (data.status) {
                        //Refresh the calender
                        FetchEventAndRenderCalendar();
                        $('#myModal').modal('hide');
                    }
                },
                error: function () {
                    alert('Não foi possivel deletar agenda');
                }
            })
        }
    })

    $('#dtp1,#dtp2').datetimepicker({
        format: 'DD/MM/YYYY HH:mm'
    });

    $('#chkIsFullDay').change(function () {
        if ($(this).is(':checked')) {
            $('#divEndDate').hide();
        }
        else {
            $('#divEndDate').show();
        }
    });

    function openEditForm() {
        $.get('/Agendas/GetAgendamento', { agendaId: selectedEvent.agendaId })
            .done((value) => {
                cadastroId = value.OrdemServico.CadastroId;
                $('#tipo').val(value.TipoAgendamento);
                $('#tipo option[value=' + value.TipoAgendamento + ']').attr('selected', 'selected');
                $('#hdAgendaId').val(value.AgendaId);
                $('#txtStart').val(selectedEvent.start.format('DD/MM/YYYY HH:mm A'));
                $('#txtDescription').val(value.Description);
                $('#ddThemeColor').val(value.Color);
                $.get('/Agendas/GetCliente/', { tipo: value.TipoAgendamento })
                    .done((data) => {
                        var itens = "<option> --Selecione Cliente-- </option>";
                        $.each(data, function (i, cli) {
                            itens += "<option value= " + cli.Value + "> " + cli.Text + " </option>";
                        });
                        $("#txtCliente").html(itens);
                        $('#txtCliente').val(cadastroId);
                    });
                $('#myModal').modal('hide');
                $('#myModalSave').modal();
            });
    }

    function openAddForm() {
        if (selectedEvent != null) {
            $('#hdAgendaId').val(selectedEvent.agendaId);
            $('#txtCliente').val(selectedEvent.title);
            $('#txtStart').val(selectedEvent.start.format('DD/MM/YYYY HH:mm A'));
            $('#txtDescription').val(selectedEvent.description);
            $('#ddThemeColor').val(selectedEvent.color);
        }
        $('#myModal').modal('hide');
        $('#myModalSave').modal();
    }

    $('#btnSave').click(function () {
        //Validation/
        if ($('#txtCliente').val().trim() == "") {
            alert('Escolha um Cliente!!');
            return;
        }
        if ($('#txtStart').val().trim() == "") {
            alert('Escolha data / hora');
            return;
        }

        var data = {
            AgendaId: $('#hdAgendaId').val(),
            Cliente: $('#txtCliente').val().trim(),
            Start: $('#txtStart').val().trim(),
            Description: $('#txtDescription').val(),
            ThemeColor: $('#ddThemeColor').val(),
            TipoAgendamento: $('#tipo').val()
        }
        SaveEvent(data);
        // call function for submit data to the server
    })

    function SaveEvent(data) {
        $.ajax({
            type: "POST",
            url: '/agendas/SaveEvent',
            data: data,
            success: function (data) {
                if (data.status) {
                    //Refresh the calender
                    FetchEventAndRenderCalendar();
                    $('#myModalSave').modal('hide');
                }
            },
            error: function () {
                setTimeout(function () {
                    swal({
                        title: "Erro",
                        text: "Não foi possivel alterar",
                        type: "error",
                        confirmButtonText: "OK"
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                window.location.href = "Agendas";
                            }
                        });
                }, 500);
            }
        })
    }
})
