(function () {
    var $content = $(".content-admin"),
        $depsSelectCont = $(".deps-select-container", $content),
        $tabContent = $(".tab-content", $content),
        $employeePromptTemplate = $(".employee-prompt-template", $tabContent),
        $employeePrompt,
        $removePrompt = $(".remove-prompt", $tabContent),
        $personalTab = $(".personal-tab", $tabContent),
        nameExp = /^\S.+\S$/,
        passwExp = /^(?=.{6,}$)[^\s]+$/;

    personalListUpdate();

    $(".personal-table", $personalTab).on("click", "tr", clickPersonalTable);
    $(".save-employee-btn", $employeePromptTemplate).click(clickSaveEmployeeBtn);
    $(".remove-employee-btn", $employeePromptTemplate).click(clickRemoveEmployeeBtn);
    $(".add-employee-btn", $personalTab).click(clickAddEmployeeBtn);
    $employeePromptTemplate.on("hidden.bs.modal", removeFromMemory);
    $(".firstname-input", $employeePromptTemplate).keyup(changeNameInput).on("input", changeNameInput);
    $(".lastname-input", $employeePromptTemplate).keyup(changeNameInput).on("input", changeNameInput);
    $(".pass-input", $employeePromptTemplate).keyup(changePassInput).on("input", changePassInput);
    $(".pass-check", $employeePromptTemplate).change(changePassCheck);
    $(".deps-select", $depsSelectCont).change(personalListUpdate);

    function clickPersonalTable(event) {
        var $currentRow = $(event.target).closest("tr"),
            employeeId = $currentRow.data("employee-id"),
            firstName = $(".firstname-td", $currentRow).text(),
            lastName = $(".lastname-td", $currentRow).text();

        $(".employee-prompt").remove();
        $employeePrompt = $employeePromptTemplate.clone(true);
        $employeePrompt.removeClass("employee-prompt-template").addClass("employee-prompt");

        $employeePrompt.data("employee-id", employeeId);
        $(".firstname-input", $employeePrompt).val(firstName);
        $(".lastname-input", $employeePrompt).val(lastName);
        $(".pass-input", $employeePrompt).val("").prop("readonly", true);
        $(".pass-check", $employeePrompt).prop("checked", false);

        $employeePrompt.modal("show");
    }

    function clickAddEmployeeBtn() {
        $(".employee-prompt").remove();
        $employeePrompt = $employeePromptTemplate.clone(true);
        $employeePrompt.removeClass("employee-prompt-template").addClass("employee-prompt");

        $employeePrompt.data("employee-id", -1);
        $(".firstname-input", $employeePrompt).val("");
        $(".lastname-input", $employeePrompt).val("");
        $(".pass-input", $employeePrompt).val("");
        $(".pass-check", $employeePrompt).prop("checked", true);

        $employeePrompt.modal("show");
    }

    function clickSaveEmployeeBtn() {
        var employeeId = $employeePrompt.data("employee-id") + "",
            firstName = $(".firstname-input", $employeePrompt).val(),
            lastName = $(".lastname-input", $employeePrompt).val(),
            password = $(".pass-input", $employeePrompt).val();

        if (!nameExp.test(lastName) || !nameExp.test(firstName)) {
            showError("Не допустимое значения полей ввода");
            return;
        }

        if (employeeId === "-1" && !$(".pass-check", $employeePrompt).prop("checked")) {
            showError("Пароль обязателен при добаление пользователя");
            return;
        }

        if (employeeId === "-1" || $(".pass-check", $employeePrompt).prop("checked")) {
            if (!passwExp.test(password)) {
                showError("Пароль должен содержать не менее 6 символов без пробелов");
                return;
            }
        } else {
            password = "";
        }

        saveEmployee(employeeId, firstName, lastName, password);
    }

    function changeNameInput(event) {
        var $input = $(event.target);
        toggleInputFeedback($input, nameExp.test($input.val()));
    }

    function changePassInput(event) {
        var $input = $(event.target);
        toggleInputFeedback($input, passwExp.test($input.val()));
    }

    function clickRemoveEmployeeBtn() {
        var employeeId = $employeePrompt.data("employee-id") + "",
            firstName = $(".firstname-input", $employeePrompt).val(),
            lastName = $(".lastname-input", $employeePrompt).val();

        if (employeeId === "-1") {
            return;
        }

        $removePrompt.modal("show");
        $employeePrompt.modal("hide");
        $(".modal-title", $removePrompt).text("Удаление");
        $(".modal-body p", $removePrompt).empty().append("Удалить <b>" + lastName + " " + firstName + "</b>?");
        $(".remove-prompt-btn", $removePrompt).click(function (event) {
            var $thisBtn = $(event.target);

            $thisBtn.button("loading");

            $.ajax({
                url: "AdminsAjax",
                method: "post",
                cache: false,
                data: {
                    queryName: "removeEmployee",
                    employeeid: employeeId
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $removePrompt.modal("hide");
                    personalListUpdate();
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function clickChangePassBtn() {
        var employeeId = $employeePrompt.data("employee-id") + "",
            firstName = $(".firstname-input", $employeePrompt).val(),
            lastName = $(".lastname-input", $employeePrompt).val();

        $removePrompt.modal("show");
        $employeePrompt.modal("hide");
        $(".modal-title", $removePrompt).text("Удаление");
        $(".modal-body p", $removePrompt).empty().append("Удалить <b>" + lastName + " " + firstName + "</b>?");
        $(".remove-prompt-btn", $removePrompt).click(function (event) {
            var $thisBtn = $(event.target);

            $thisBtn.button("loading");

            $.ajax({
                url: "AdminsAjax",
                method: "post",
                cache: false,
                data: {
                    queryName: "removeEmployee",
                    employeeid: employeeId
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $removePrompt.modal("hide");
                    personalListUpdate();
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function changePassCheck(event) {
        $(".pass-input", $employeePrompt).prop("readonly", !$(event.target).prop("checked"));
    }

    function saveEmployee(employeeId, firstName, lastName, password) {
        var requestOwner = $content.data("user-id") + "";

        $(".save-employee-btn", $employeePrompt).button("loading");

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            cache: false,
            data: {
                queryName: "saveEmployeeByDepFromOwner",
                requestowner: requestOwner,
                employeeid: employeeId,
                firstname: firstName,
                lastname: lastName,
                password: password
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                $employeePrompt.modal("hide");
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $(".save-employee-btn", $employeePrompt).button("reset");
            personalListUpdate();
        });
    }

    function personalListUpdate() {
        var requestOwner = $content.data("user-id") + "",
            depId = $(".deps-select", $depsSelectCont).val();

        if (depId === undefined) {
            depId = -1;
        }

        $.ajax({
            url: "RegularAjax",
            method: "get",
            cache: false,
            data: {
                queryName: "listEmployeesByDep",
                requestowner: requestOwner,
                depid: depId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                populatePersonalTable(result.Data);
            } else {
                showError(result.Error);
            }
        });
    }

    function populatePersonalTable(data) {
        var $row;

        $(".personal-table tbody", $personalTab).empty();

        if (data !== null) {
            $(data).each(function (index, el) {
                $row = $("<tr data-employee-id=\"" + el.Id + "\"></tr>").clone();
                $row.append("<td class='lastname-td'>" + el.LastName + "</td>");
                $row.append("<td class='firstname-td'>" + el.FirstName + "</td>");
                $(".personal-table tbody", $personalTab).append($row);
            });
        }
    }

    function removeFromMemory(event) {
        //$(event.target).remove();
        //$(".modal-backdrop").remove();
        $(this).data("modal", null);
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
}());