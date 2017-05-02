$(function () {
    $.get("http://localhost:62664/api/RTB", function (data, status) {
        $("#auctionAd").html(data);
    });
});