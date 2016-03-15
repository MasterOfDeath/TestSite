(function () {
    var $content = $(".content-photos"),
        $thumbnails = $(".thumbnail", $content),
        $editPrompt = $(".edit-prompt", $content),
        $removePrompt = $(".remove-prompt", $content),
        $editPromptInput = $("input[type='text']", $editPrompt),
        photoId;

    $thumbnails.on("click", ".remove-btn", clickRemoveBtn);
    $thumbnails.on("click", ".edit-btn", clickEditBtn);
    $thumbnails.on("click", ".photo-like-notowner", clickLikeBtn);
    $thumbnails.on("click", ".photo-like-owner", clickLikeOwnerBtn);

    $(".fancybox").fancybox({
        openEffect: "none",
        closeEffect: "none",
        type: "image",
        loop: false,
        fixed: false,
        autoCenter: false
    });

    function clickEditBtn(e) {
        e.stopPropagation();
        e.preventDefault();
        var $thisThumb = $(e.target).closest(".thumbnail");

        $editPromptInput.val($(".photo-name-container > .photo-name", $thisThumb).text());
        $(".modal-title", $editPrompt).text("Edit Photo");
        photoId = $thisThumb.data("photo-id") + "";
        $editPrompt.modal();
    }

    function clickRemoveBtn(e) {
        e.stopPropagation();
        e.preventDefault();
        var $thisThumb = $(e.target).closest(".thumbnail");

        $(".modal-body", $removePrompt).html("Do you really want to delete photo "
            + "<b>" + $(".photo-name-container > .photo-name", $thisThumb).text() + "</b>?");

        $(".modal-title", $removePrompt).text("Remove Photo");
        photoId = $thisThumb.data("photo-id") + "";
        $removePrompt.modal();
    }

    function clickLikeBtn(e) {
        e.stopPropagation();
        e.preventDefault();
        var $thisThumb = $(e.target).closest(".thumbnail");

        photoId = $thisThumb.data("photo-id") + "";
        
        $.ajax({
            url: 'UsersAjax',
            method: 'post',
            data: {
                queryName: "clickLikeBtn",
                photoid: photoId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                $(".badge", $thisThumb).text(" " + result.Data);
            } else {
                showError(result.Error);
            }
        })
    }

    function clickLikeOwnerBtn(e) {
        e.stopPropagation();
        e.preventDefault();

        showError("You cann't like your own photo");
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
                    queryName: "clickPromptEditPhotoBtn",
                    photoid: photoId,
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
                queryName: "clickPromptRemovePhotoBtn",
                photoid: photoId
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

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }

}());