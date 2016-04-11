(function () {
    var $content = $(".content-login"),
        $depSelector = $(".dep-selector", $content),
        $employeeSelector = $(".employee-selector", $content);

    $depSelector.change(changeDepSelector);
    $(".login-btn", $content).click(clickLoginBtn);

    // Для отрисовки $employeeSelector
    changeDepSelector();

    function changeDepSelector() {
        var depId = $depSelector.val();

        $employeeSelector.prop("disabled", true);

        $.ajax({
            url: "RegularAjax",
            method: "post",
            data: {
                queryName: "changeDepSelector",
                depid: depId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                $employeeSelector.empty();
                $employeeSelector.append($(result.Data));
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $employeeSelector.prop("disabled", false);
        });
    }

    function clickLoginBtn() {
        var employeeId = $employeeSelector.val(),
            password = $(".password-input", $content).val(),
            remember = $(".password-remember", $content).prop("checked");

        $(".login-btn", $content).button("loading");
        $(".wrong-password-alert", $content).fadeOut(0);

        $.ajax({
            url: "SignIn",
            method: "post",
            data: {
                queryName: "clickLoginBtn",
                employeeid: employeeId,
                password: password,
                remember: remember
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                if (result.Data.login) {
                    window.location.replace(getRedirectUrl());
                }
                else {
                    $(".wrong-password-alert", $content).fadeIn("slow");
                }
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $(".login-btn", $content).button("reset");
        });
    }

    function getRedirectUrl() {
        var url = window.location.toString(),
            match;

        match = /ReturnUrl=(.+)&/.exec(url);

        if (match === null)
        {
            return "/";
        }
        else {
            return decodeURIComponent(match[1]);
        }
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());