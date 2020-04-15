$(document).ready(function () {
    SetUpDashbaord();
    $("#mdi-menu,#mdi-arrow-expand-all").click(function () {
        setTimeout(function () {
            SetUpDashbaord()
        }, 100);
    }); 
    $(".jump").click(function () {
        var url = $(this).attr("dashboard");
        $('#dashbaord').attr("src", url); 
    }); 
});
function SetUpDashbaord() {
    $('#dashbaord').height("0px");
    var position = $(window).height() - 70 - 10;
    $('#dashbaord').height(position + "px");
}


