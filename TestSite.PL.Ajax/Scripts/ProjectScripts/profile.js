(function () {
    var $content = $(".content-profile"),
        $nameInput = $(".name-input", $content),
        $alertSuccess = $(".alert-success", $content),
        $firstNameInput = $(".tab-content .firstname-input", $content),
        $lastNameInput = $(".tab-content .lastname-input", $content),
        $oldPasswordInput = $(".tab-content .old-password-input", $content),
        $newPasswordInput = $(".tab-content .new-password-input", $content),
        $secBodyTemplate = $(".sec-body-template", $content),
        $secPrompt = $(".security-prompt", $content),
        $thisRow = null,
        nameExp = /[^\w\- \.]+|[ ]{2,}|[\-\.]{2,}|^[\-\.]+$|^[ \-\.]| $/,
        passwExp = /^(?=.{6,}$)([^\s]*[a-zA-Z0-9][^\s]*)+$/,
        userId = $content.data("user-id") + "",
        thisUserId =null,
        usersRoles = null,
        enabled = null;

    $(".userinfoTab input[type=text]", $content).change(changeNameInput);
    $(".userinfoTab input[type=text]", $content).keyup(changeNameInput);
    $(".passwordTab input[type=password]", $content).keyup(changePasswordInput);

    function changeNameInput(event) {
        var $input = $(event.target),
            str = $input.val() + "";
        toggleInputError($input, !isValidName(str));
    }

    function changePasswordInput(event) {
        var $input = $(event.target),
            str = $input.val() + "";
        toggleInputError($input, str.search(passwExp) === -1);
    }

    $(".saveProfileBtn", $content).click(function (e) {
        var $thisBtn = $(e.target),
            firstname = $firstNameInput.val(),
            lastname = $lastNameInput.val();

        if (isValidName(firstname) && isValidName(lastname) && +userId > 0) {
            $thisBtn.button("loading");

            $.ajax({
                url: "UsersAjax",
                method: "post",
                data: {
                    queryName: "clickSaveProfileBtn",
                    userid: userId,
                    firstname: firstname,
                    lastname: lastname
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $alertSuccess.removeClass("hide")
                        .children("p")
                        .html("Profile info successefully updated");
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        } else {
            showError("Invalid values of FirstName or LastName");
        }
    });

    $(".changePasswordBtn", $content).click(function (e) {
        var $thisBtn = $(e.target),
            oldPassword = $oldPasswordInput.val(),
            newPassword = $newPasswordInput.val();

        if (oldPassword.search(passwExp) > -1 &&
            newPassword.search(passwExp) > -1 &&
            +userId > 0)
        {
            $thisBtn.button("loading");

            $.ajax({
                url: "UsersAjax",
                method: "post",
                data: {
                    queryName: "clickChangePasswordBtn",
                    userid: userId,
                    oldpassword: oldPassword,
                    newpassword: newPassword
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $alertSuccess.removeClass("hide")
                        .children("p")
                        .html("Password was successefully changed");
                    $oldPasswordInput.val("");
                    $newPasswordInput.val("");
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        } else {
            showError("Invalid values of Old Password or New Password");
        }
    });

    // OnClick on row in table
    $(".table tbody", $content).on("click", "tr > td", function (e) {
        var $thisTemplate = $secBodyTemplate.clone();

        $thisRow = $(e.target).parent();
        usersRoles = $thisRow.children(".roles").text().split(", ");
        enabled = $thisRow.children(".enabled").text().toLowerCase();
        thisUserId = $thisRow.data("user-id") + "";

        if (usersRoles == null || enabled == null || +thisUserId < 1) {
            showError("Incorrect data");
            return;
        }

        $(".roles-control input[type=checkbox]", $thisTemplate).each(function (index, el) {
            if ($.inArray($(el).val(), usersRoles) > -1) {
                $(el).prop("checked", true);
            } else {
                $(el).prop("checked", false);
            }
        });

        if (enabled === "true") {
            $(".state-control input[type=checkbox]", $thisTemplate).prop('checked', true);
        } else {
            $(".state-control input[type=checkbox]", $thisTemplate).prop('checked', false);
        }

        $(".modal-body .to-replace", $secPrompt).replaceWith($thisTemplate.addClass("to-replace").removeClass("hide"));

        $(".modal-title", $secPrompt).html($thisRow.children(".username").text());

        $secPrompt.modal("show");
    });

    $(".security-prompt-btn", $secPrompt).click(function (e) {
        var roleChanges = [],
            enableChange = null,
            enableState;

        $(".roles-control input[type=checkbox]", $secPrompt).each(function (index, el) {
            if ($.inArray($(el).val(), usersRoles) === -1 && !!$(el).prop("checked")) {
                roleChanges.push({ role: $(el).val(), action: true });
            }

            if ($.inArray($(el).val(), usersRoles) > -1 && !$(el).prop("checked")) {
                roleChanges.push({ role: $(el).val(), action: false });
            }
        });

        enableState = $(".state-control input[type=checkbox]", $secPrompt).prop("checked") + "";

        if (enableState.toLowerCase !== enabled) {
            enableChange = $(".state-control input[type=checkbox]", $secPrompt).prop("checked");
        }

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: {
                queryName: "clickSecurityPromptBtn",
                userid: thisUserId,
                rolechanges: JSON.stringify(roleChanges),
                enablechange: JSON.stringify(enableChange)
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                getNewRow();
            } else {
                showError(result.Error);
            }
        });
    });

    $(".alert-success .close", $content).on("click", function (e) {
        $(this).parent().addClass("hide");
    });

    function toggleInputError($el, show) {
        var $container = $el.parents(".has-feedback");

        if (show) {
            $container.removeClass("has-success").addClass("has-error");
            $(".badIcon", $container).removeClass("hide");
            $(".goodIcon", $container).addClass("hide");
        } else {
            $container.removeClass("has-error").addClass("has-success");
            $(".badIcon", $container).addClass("hide");
            $(".goodIcon", $container).removeClass("hide");
        }
    }

    function getNewRow() {
        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: {
                queryName: "getHtmlForUsersSecurityTable",
                userid: thisUserId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {

                $thisRow.replaceWith(result.Data);
                $secPrompt.modal("hide");

            } else {
                showError(result.Error);
            }
        });
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }

    function isValidName(str) {
        return (str.length >= 1 && (str.search(nameExp) === -1))
    }
}());