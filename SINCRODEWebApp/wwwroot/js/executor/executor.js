(function () {

    $("#fechaWarning").hide();

    var fechaInicio = null;
    var fechaFin = null;

    $("#fechaInicio").on("change", function (event) {
        fechaInicio = event.target.valueAsNumber;
        validatorWarning(fechaInicio, fechaFin);
    });

    $("#fechaFin").on("change", function (event) {
        fechaFin = event.target.valueAsNumber;
        validatorWarning(fechaInicio, fechaFin);
    });

    $("#executor").on("click", function (event) {
        var fi = $("#fechaInicio").val();
        var ff = $("#fechaFin").val();

        var submit = validatorWarning(fi, ff);

        if (submit) {
            $(".modal-dialog").fadeOut("slow");
            $("#loading").fadeIn();
            var opts = {
                lines: 10, // The number of lines to draw
                length: 8, // The length of each line
                width: 3, // The line thickness
                radius: 7, // The radius of the inner circle
                scale: 0.95, // Scales overall size of the spinner
                corners: 1, // Corner roundness (0..1)
                color: '#ffffff', // CSS color or array of colors
                fadeColor: 'transparent', // CSS color or array of colors
                speed: 0.6, // Rounds per second
                rotate: 33, // The rotation offset
                animation: 'spinner-line-shrink', // The CSS animation name for the lines
                direction: 1, // 1: clockwise, -1: counterclockwise
                zIndex: 2e9, // The z-index (defaults to 2000000000)
                className: 'spinner', // The CSS class to assign to the spinner
                top: '50%', // Top position relative to parent
                left: '50%', // Left position relative to parent
                shadow: false, // Box-shadow for the lines
                position: 'absolute' // Element positioning
            };
            var target = document.getElementById('loading');
            var spinner = new Spinner(opts).spin(target);
            $("#executorForm").submit();
        }
    });
})();

function validatorWarning(fechaInicio, fechaFin) {

    if (fechaInicio === fechaFin) {

        if (!fechaInicio || !fechaFin) {
            $("#fechaWarning").show("slow");
            return false;
        }

        $("#fechaWarning").hide("slow");
        return true;
    }

    if (!fechaInicio || !fechaFin) {
        $("#fechaWarning").show("slow");
        return false;
    }

    if (fechaInicio > fechaFin) {
        $("#fechaWarning").show("slow");
        return false;
    }

    $("#fechaWarning").hide("slow");

    return true;
}

function onSubmitValidator() {
    var fechaInicio = $("#fechaInicio").val();
    var fechaFin = $("#fechaFin").val();

    if (!fechaInicio || !fechaFin) {
        $("#fechaWarning").show("slow");
        return;
    }

    if (fechaInicio > fechaFin) {

        $("fechaWarning").show("slow");
        return false;
    }

    $("fechaWarning").hide("fast");
    $("#loading").fadeIn();
    return true;
}