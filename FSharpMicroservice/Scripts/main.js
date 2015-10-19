function sendNotification() {
    var uri = "notification";
    var request = {
        UserId: $("#UserId").val(),
        Message: $("#Message").val()
    };

    $.post(uri, request)
        .done(function(data) {
            alert("Notification sent.");
        });
}