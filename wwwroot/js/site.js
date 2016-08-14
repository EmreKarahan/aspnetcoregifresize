var App = function () {

    var testHubProxy = $.connection.testHub;

    return {
        getMessage: function (message) {
            if (message != null) {
                $(".shell-body").append("<li>" + message + "</li>");
                $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
            }
        },

        getError: function (message) {
            if (message != null) {
                $(".shell-body").append("<li class='error'>" + message + "</li>");
                $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
            }
        },

        getExited: function (message) {
            if (message != null) {
                $(".shell-body").append("<li class='finish'>" + message + "</li>");
                $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
            }
        },

        clearScreen: function () {
            $(".shell-body").append("<li class='info'>Now connected Encoder Server, Your Connection ID=" + $.connection.hub.id + "</li>");
            $(".shell-body").append("<li class='info'>Ready...</li>");
            $(".shell-body").animate({ scrollTop: $('.shell-body').prop("scrollHeight") }, 1);
        },

        addSelectedFileName: function () {
            $(".shell-body").append("<li class='info'>Selected File => '" + this.value + "'" + "</li>");
        },

        gifProcess: function () {
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
        },

        uploadFile: function (evt) {

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
                        $('.progress-bar').css('width', progressBarValue + '%').attr('aria-valuenow', progressBarValue);

                        $(".shell-body").append("<li> </li>");
                        if (progressBarValue != 100) {
                            $(".shell-body li:last").text("Uploading " + progressBarValue + "%");

                        }
                        else {
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
        },

        init: function () {
            self = this;

            $.connection.hub.start()
                .done(function () {
                    self.clearScreen();
                })
                .fail(function () { console.log('Could not Connect!'); });

            testHubProxy.client.message = self.getMessage;
            testHubProxy.client.error = self.getError;
            testHubProxy.client.exited = self.getExited;

            $("#upload").on("click", self.uploadFile);
            $("#files").on("change", self.addSelectedFileName);
            $("#gif-process-button").on("click", self.gifProcess);

        }
    }
} ();

App.init();