var testHubProxy = $.connection.testHub;
testHubProxy.client.message = function (message) {

    if (message != null) {
        $(".shell-body").append("<li>" + message + "</li>");
        $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
    }
};


testHubProxy.client.error = function (message) {

    if (message != null) {
        $(".shell-body").append("<li class='error'>" + message + "</li>");
        $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
    }
};


testHubProxy.client.exited = function (message) {

    if (message != null) {
        $(".shell-body").append("<li class='finish'>" + message + "</li>");
        $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
    }
};


$.connection.hub.start()
    .done(function () {
        clearScreen();
    })
    .fail(function () { console.log('Could not Connect!'); });



function clearScreen() {
    $(".shell-body").append("<li class='info'>Now connected Encoder Server, Your Connection ID=" + $.connection.hub.id + "</li>");
    $(".shell-body").append("<li class='info'>Ready...</li>");
    $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
}


$(document).ready(function () {

    $("#upload").click(function (evt) {
        //$(".shell-body").empty();
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
                    //console.log(Math.floor(e.loaded / e.total * 100) + '%');
                    //console.log(progressBarValue);
                    $('.progress-bar').css('width', progressBarValue + '%').attr('aria-valuenow', progressBarValue);

                    $(".shell-body").append("<li> </li>");
                    if (progressBarValue != 100) {
                        //$("li:last").empty();
                        $(".shell-body li:last").text("Uploading " + progressBarValue + "%");

                    }
                    else {
                        //$(".shell-body").empty();
                        $(".shell-body li:last").text("Uploading " + progressBarValue + "%");
                        $(".shell-body").append("<li>Upload Completed.</li>");
                    }
                    $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
                };
                return xhr;
            },

            type: "POST",
            url: "/Home/UploadFile",
            contentType: false,
            processData: false,
            data: data,
            success: function (data) {
                $("#uploaded").attr("src", data.path);
                $("#gif-upload-panel").hide();
                $("#gif-encode-panel").show();
                $("#gif-file-name").val(data.fileName);
                console.log(data);
            },
            error: function () {
                console.log("error");
            }
        });
    });


    $("#gif-process-button").on("click", function () {
        var fileName = $("#gif-file-name").val();
        $.ajax({
            type: "GET",
            url: "/Home/ProcessGif",
            data: {
                'fileName': fileName
            },
            success: function (data) {
                console.log(data);
            },
            error: function () {
                console.log("error");
            }
        });
    });

    $("#files").on("change", function () {
        $(".shell-body").append("<li class='info'>Selected File => '" + this.value + "'" + "</li>");
    });

});