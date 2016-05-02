(function () {
    var $content = $(".content-profile"),
        passwExp = /^(?=.{6,}$)[^\s]+$/;

    $(".new-password-input", $content).keyup(changeInput);
    $(".changePasswordBtn", $content).click(clickChangePasswordBtn);

    function changeInput(event) {
        var $input = $(event.target);
        toggleInputFeedback($input, passwExp.test($input.val()));
    }

    function clickChangePasswordBtn() {
        var employeeId = $content.data("user-id"),
            oldPassword = $(".old-password-input", $content).val(),
            newPassword = $(".new-password-input", $content).val();

        if (+employeeId < 0) {
            showError("Не известный пользователь");
            return;
        }

        if (oldPassword == "" && !passwExp.test(newPassword)) {
            showError("Пароли должны содержать не менее 6 символов без пробелов");
            return;
        }

        $(".changePasswordBtn", $content).button("loading");

        $.ajax({
            url: "UsersAjax",
            method: "post",
            cache: false,
            data: {
                queryName: "changePassword",
                employeeid: employeeId,
                oldpassword: oldPassword,
                newpassword: newPassword
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                if (result.Data) {
                    $(".notify-container .alert-success").removeClass("hide");
                    $(".passwordTab :password").val("");
                } else {
                    showError("Пароль не был изменён");
                }
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $(".changePasswordBtn", $content).button("reset");
        });
    }

    function toggleInputFeedback($el, show) {
        var $container = $el.parents(".has-feedback");

        if (show) {
            $container.removeClass("has-error").addClass("has-success");
            $(".form-control-feedback", $container).removeClass("glyphicon-warning-sign");
            $(".form-control-feedback", $container).addClass("glyphicon-ok");
        } else {
            $container.removeClass("has-success").addClass("has-error");
            $(".form-control-feedback", $container).removeClass("glyphicon-ok");
            $(".form-control-feedback", $container).addClass("glyphicon-warning-sign");
        }
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
})();