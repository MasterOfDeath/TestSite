(function () {
    var $content = $(".content-login"),
        $depSelect = $(".dep-select", $content),
        $employeeSelect = $(".employee-select", $content),
        usersRoleId = 2;

    $depSelect.change(changeDepSelector);
    $employeeSelect.change(changeEmployeeSelect)
    $(".login-btn", $content).click(clickLoginBtn);
    $(".password-input", $content).keyup(onEnterPress);

    // Для отрисовки $employeeSelect
    changeDepSelector();

    function changeDepSelector() {
        var depId = $depSelect.val();

        $employeeSelect.prop("disabled", true);

        $.ajax({
            url: "RegularAjax",
            method: "get",
            cache: false,
            data: {
                queryName: "listEmployeesByDep",
                depid: depId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                populateEmployeeSelector(result.Data);
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $employeeSelect.prop("disabled", false);
        });
    }

    function changeEmployeeSelect(event) {
        var roleId = $(event.target).find('option:selected').data("role-id");

        $(".password-input", $content).prop("readonly", +roleId === usersRoleId);
    }

    function clickLoginBtn() {
        var employeeId = $employeeSelect.val(),
            password = $(".password-input", $content).val(),
            remember = $(".password-remember", $content).prop("checked");

        $(".login-btn", $content).button("loading");
        $(".wrong-password-alert", $content).fadeOut(0);

        $.ajax({
            url: "SignIn",
            method: "post",
            cache: false,
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

    function onEnterPress(event) {
        if (event.keyCode == 13) {
            clickLoginBtn();
        }
    }

    function populateEmployeeSelector(data) {
        $employeeSelect.empty();

        if (data !== null) {
            $(data).each(function (index, el) {
                $employeeSelect.append("<option data-role-id='" + el.Role_Id +"' value=" + el.Id + ">" + el.LastName + " " + el.FirstName + "</option>");
            });

            $employeeSelect.change();
        }
    }

    function getRedirectUrl() {
        var url = window.location.toString(),
            match;

        match = /ReturnUrl=(.+)(?:&|$)/.exec(url);

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