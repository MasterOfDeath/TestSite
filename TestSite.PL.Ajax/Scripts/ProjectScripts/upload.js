(function () {
    var $content = $(".content-upload"),
        $nameInput = $(".name-input", $content),
        $uploadBtn = $(".upload-btn", $content),
        $imgInput = $(".image-input", $content),
        $albumSelect = $(".album-select", $content),
        $imageForm = $('.image-upload-form', $content),
        $bar = $(".bar", $content),
        $percent = $(".percent", $content),
        imageMaxSize = 4000000,
        albumId;

    $uploadBtn.click(clickUploadBtn);
    $imgInput.change(changeInputImage);
    $albumSelect.change(changeAlbumSelect);

    albumId = $albumSelect.val();

    $(".alert-success .close", $content).on("click", function (e) {
        $(this).parent().addClass("hide");
    });

    function clickUploadBtn() {
        var name = $nameInput.val();

        $(".albumid", $imageForm).val(albumId);
        $(".name", $imageForm).val($nameInput.val());
        $(".size", $imageForm).val($imgInput.get(0).files[0].size);

        if (name.length <= 0) {
            showError("The name mustn't be empty");
            return;
        }

        if ($imgInput.get(0).files[0] === null || $imgInput.get(0).files[0] === undefined) {
            showError("You have to select a photo");
        }

        $bar.closest(".form-group").removeClass("hide");

        $imageForm.submit();
    }

    function changeInputImage(event) {
        var input = event.target;

        if (input.files && input.files[0]) {
            if (input.files[0].size > imageMaxSize) {
                showError("Max size is: " + imageMaxSize + "Kb but your file is:" + input.files[0].size + "Kb");
                input.value = "";
                $(".image-container > img", $content).attr("src", "/images/DefaultPhoto.png");
            } else {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $(".image-container > img", $content).attr("src", e.target.result);
                }

                reader.readAsDataURL(input.files[0]);

                $nameInput.val(input.files[0].name);
            }
        }
    }

    function onUploadSuccess() {
        $(".alert-success", $content).removeClass("hide");
        $(".alert-success i", $content).html("Photo <b>" + $nameInput.val() + "</b> has been added successfully");

        $imgInput.get(0).value = "";
        $nameInput.get(0).value = "";
        $(".image-container > img", $content).attr("src", "/images/DefaultPhoto.png");

        $bar.closest(".form-group").addClass("hide");
    }

    function changeAlbumSelect() {
        albumId = $albumSelect.val();
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }

    $imageForm.ajaxForm({
        beforeSend: function () {
            var percentVal = "0%";
            $bar.width(percentVal);
            $percent.html(percentVal);
            $uploadBtn.button("loading");
        },
        uploadProgress: function (event, position, total, percentComplete) {
            var percentVal = percentComplete + "%";
            $bar.width(percentVal);
            $percent.html(percentVal);
        },
        success: function () {
            var percentVal = "100%";
            $bar.width(percentVal);
            $percent.html(percentVal);
        },
        complete: function (data) {
            var result = JSON.parse(data.responseText);

            $uploadBtn.button("reset");

            if (result.Error === null) {
                onUploadSuccess();
            } else {
                showError(result.Error);
            }
        }
    });

}());