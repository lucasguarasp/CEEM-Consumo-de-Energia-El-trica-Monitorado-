$("#tipo").change(function () {
    var tipo = $("#tipo").val();
    $.ajax({
        url: '/Agendas/GetCliente/',
        type: 'POST',
        dataType: 'json',
        data: {
            tipo: tipo
        },
        success: function (data) {
            var itens = "<option> --Selecione Cliente-- </option>";
            $.each(data, function (i, cli) {
                itens += "<option value= " + cli.Value + "> " + cli.Text + " </option>";
            });
            $("#txtCliente").html(itens);
        }

    });
});