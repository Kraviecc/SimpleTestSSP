$(function () {
    $.get("http://localhost:62664/api/RTB", function (data, status) {
        if (data) {
            $("#auctionAd").attr('src', 'data:image/png;base64, ' + data);
        }
        
        $("#auctionAd").css('visibility', 'visible');
    });
});