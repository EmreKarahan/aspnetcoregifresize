var testHubProxy = $.connection.testHub;
testHubProxy.client.hello = function (message) {

    if (message != null) {
        $(".shell-body").append("<li>" + message + "</li>");
        $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
    }
};

testHubProxy.client.exited = function (message) {

    if (message != null) {
        $(".shell-body").append("<li>" + message + "</li>");
        $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
    }
};


$.connection.hub.start()
    .done(function () {
        console.log('Now connected, connection ID=' + $.connection.hub.id);
        // Wire up Send button to call NewContosoChatMessage on the server.
        $('#newContosoChatMessage').click(function () {
            testHubProxy.server.newContosoChatMessage($('#displayname').val(), $('#message').val());
            $('#result').val('').focus();
        });
    })
    .fail(function () { console.log('Could not Connect!'); });






$(document).ready(function () {
    $("#upload").click(function (evt) {
        $(".shell-body").empty();
        var fileUpload = $("#files").get(0);
        var files = fileUpload.files;
        var data = new FormData();
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
        }
        $.ajax({
            xhr: function () {
                var xhr = $.ajaxSettings.xhr();
                xhr.upload.onprogress = function (e) {
                    var progressBarValue = (Math.floor(e.loaded / e.total * 100));
                    console.log(Math.floor(e.loaded / e.total * 100) + '%');
                    console.log(progressBarValue);
                    $('.progress-bar').css('width', progressBarValue + '%').attr('aria-valuenow', progressBarValue);
                };
                return xhr;
            },

            type: "POST",
            url: "/Home/UploadFile",
            contentType: false,
            processData: false,
            data: data,
            success: function (message) {
                //alert(message);
                console.log(message);
            },
            error: function () {
                // alert("There was error uploading files!");
                console.log("error");
            }
        });
    });
});