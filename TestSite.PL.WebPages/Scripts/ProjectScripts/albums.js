(function () {
    var $content = $(".content-albums"),
        $nameInput = $(".name-input", $content),
        $newAlbumSaveBtn = $(".new-album-save-btn", $content),
        $thumbnails = $(".thumbnail", $content),
        $editPrompt = $(".edit-prompt", $content),
        $removePrompt = $(".remove-prompt", $content),
        $editPromptInput = $("input[type='text']", $editPrompt),
        nameExp = /[^\w\- \.]+|[ ]{2,}|[\-\.]{2,}|^[\-\.]+$|^[ \-\.]| $/,
        userId = $content.data("user-id") + "",
        albumId;

    $nameInput.change(changeNameInput);
    $nameInput.keyup(changeNameInput);

    $newAlbumSaveBtn.click(clickNewAlbumSaveBtn);

    $thumbnails.on("click", ".remove-btn", clickRemoveBtn);
    $thumbnails.on("click", ".edit-btn", clickEditBtn);

    function changeNameInput(event) {
        var $input = $(event.target),
            str = $input.val() + "";
        
        toggleInputError($input, !isValidName(str));
    }

    function clickNewAlbumSaveBtn() {
        var name = $nameInput.val();

        if (!isValidName(name)) {
            toggleInputError($nameInput, true);
            showError("Invalid input");
            return;
        }

        $.ajax({
            url: 'UsersAjax',
            method: 'post',
            data: {
                queryName: "clickNewAlbumSaveBtn",
                userid: userId,
                name: name
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                window.location.reload();
            } else {
                showError(result.Error);
            }
        })
    }

    $(".edit-prompt-btn", $editPrompt).click(function (e) {
        var $thisBtn = $(e.target),
            name = $editPromptInput.val();

        if (name === null || name === undefined || name.length <= 0) {
            showError("Please enter new name.");
        } else {
            $thisBtn.button("loading");

            $.ajax({
                url: 'UsersAjax',
                method: 'post',
                data: {
                    queryName: "clickPromptEditAlbumBtn",
                    albumid: albumId,
                    name: name
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    window.location.reload();
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
                $editPrompt.modal("hide");
            });
        }
    });

    $(".remove-prompt-btn", $removePrompt).click(function (e) {
        var $thisBtn = $(e.target);

        $thisBtn.button("loading");

        $.ajax({
            url: 'UsersAjax',
            method: 'post',
            data: {
                queryName: "clickPromptRemoveAlbumBtn",
                albumid: albumId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                window.location.reload();
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $thisBtn.button("reset");
            $removePrompt.modal("hide");
        });
    });

    function clickEditBtn(e) {
        e.preventDefault();
        var $thisThumb = $(e.target).closest(".thumbnail");

        $editPromptInput.val($(".album-name > b", $thisThumb).text());
        $(".modal-title", $editPrompt).text("Edit Album");
        albumId = $thisThumb.data("album-id") + "";
        $editPrompt.modal();
    }

    function clickRemoveBtn(e) {
        e.preventDefault();
        var $thisThumb = $(e.target).closest(".thumbnail");

        $(".modal-body", $removePrompt).html("Do you really want to delete album "
            + "<b>" + $(".album-name > b", $thisThumb).text() + "</b>" +
            "? This will delete all the photos in it.");

        $(".modal-title", $removePrompt).text("Remove Album");
        albumId = $thisThumb.data("album-id") + "";
        $removePrompt.modal();
    }

    function toggleInputError($el, show) {
        var $container = $el.parents(".has-feedback");

        if (show) {
            $container.removeClass("has-success").addClass("has-error");
        } else {
            $container.removeClass("has-error").addClass("has-success");
        }
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }

    function isValidName(str) {
        return (str.length >= 0 && (str.search(nameExp) === -1))
    }
}());