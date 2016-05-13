(function () {
    var $content = $(".content-admin"),
        $navTabs = $(".nav-tabs", $content),
        $tabContent = $(".tab-content", $content),
        $namePrompt = $(".name-prompt", $tabContent),
        $removePrompt = $(".remove-prompt", $tabContent),
        $employeePromptTemplate = $(".employee-prompt-template", $tabContent),
        $employeePrompt,
        $depsTab = $(".deps-tab", $tabContent),
        $editDepTab = $(".editDep-tab", $tabContent),
        instpectorsDepId = 2,
        superadminsDepId = 1,
        usersRoleId = 2,
        nameExp = /^\S.+\S$/,
        passwExp = /^(?=.{6,}$)[^\s]+$/;

    depsListUpdate();

    $(".addDepBtn", $depsTab).click(clickAddDepBtn);
    $(".addEmployeeBtn", $editDepTab).click(clickAddEmployeeBtn);
    $(".editDepBtn", $editDepTab).click(clickEditDepBtn);
    $(".removeDepBtn", $editDepTab).click(clickRemoveDepBtn);
    $(".deps-table", $depsTab).on("click", "tr", clickDepsTable);
    $(".employees-table", $editDepTab).on("click", "tr", clickEmployeeTable);
    $employeePromptTemplate.on("hidden.bs.modal", removeFromMemory);
    $(".save-employee-btn", $employeePromptTemplate).click(clickSaveEmployeeBtn);
    $(".remove-employee-btn", $employeePromptTemplate).click(clickRemoveEmployeeBtn);
    $(".firstname-input", $employeePromptTemplate).keyup(changeNameInput).on("input", changeNameInput);
    $(".lastname-input", $employeePromptTemplate).keyup(changeNameInput).on("input", changeNameInput);
    $(".pass-input", $employeePromptTemplate).keyup(changePassInput).on("input", changePassInput);
    $(".pass-check", $employeePromptTemplate).change(changePassCheck);
    $(".role-select", $employeePromptTemplate).change(changeRoleSelect);

    function clickAddDepBtn() {
        var employeeId = $content.data("user-id");

        $namePrompt.modal("show");
        $(".modal-title", $namePrompt).text("Создание отдела");
        autosize($(".modal-body .name-input", $namePrompt));
        $(".modal-body .name-input", $namePrompt).val("");
        $(".save-name-btn", $namePrompt).unbind("click").bind("click", { depId: -1 }, clickSaveDep);
    }

    function clickEditDepBtn() {
        var depName = $editDepTab.data("dep-name"),
            depId = $editDepTab.data("dep-id");

        $namePrompt.modal("show");
        $(".modal-title", $namePrompt).text("Редактирование отдела");
        autosize($(".modal-body .name-input", $namePrompt));
        $(".modal-body .name-input", $namePrompt).val(depName);
        updateSizeTextArea($(".modal-body .name-input", $namePrompt), 500);
        $(".save-name-btn", $namePrompt).unbind("click").bind("click", { depId: depId }, clickSaveDep);
    }

    function clickRemoveDepBtn() {
        var depName = $editDepTab.data("dep-name"),
            depId = $editDepTab.data("dep-id");

        $removePrompt.modal("show");
        $(".modal-title", $removePrompt).text("Удаление отдела");
        $(".modal-body p", $removePrompt)
            .empty()
            .append("<b>Внимание!</b> будут удалены все пользователи и тесты данного отдела<p><b>" + depName + "</b></p>");
        $(".remove-prompt-btn", $removePrompt).click(function (event) {
            var $thisBtn = $(event.target);

            $thisBtn.button("loading");

            $.ajax({
                url: "AdminsAjax",
                method: "post",
                cache: false,
                data: {
                    queryName: "removeDep",
                    depid: depId
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $removePrompt.modal("hide");
                    depsListUpdate();
                    $(".editDep", $navTabs).addClass("hide");
                    $(".listDeps > a", $navTabs).tab("show");
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function clickSaveDep(event) {
        var $thisBtn = $(event.target),
            depId = event.data.depId;
            depName = $(".name-input", $namePrompt).val(),
            employeeId = $content.data("user-id") + "";

        if (!nameExp.test(depName)) {
            showError("Недопустимое имя");
            return;
        }

        $thisBtn.button("loading");

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            cache: false,
            data: {
                queryName: "insertDep",
                depid: depId,
                depname: depName,
                employeeid: employeeId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                depsListUpdate();
                $(".editDep", $navTabs).addClass("hide");
                $(".listDeps > a", $navTabs).tab("show");
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $namePrompt.modal("hide");
            $(".name-input", $namePrompt).val("");
            $thisBtn.button("reset");
        });
    }

    function changeNameInput(event) {
        var $input = $(event.target);
        toggleInputFeedback($input, nameExp.test($input.val()));
    }

    function changePassInput(event) {
        var $input = $(event.target);
        toggleInputFeedback($input, passwExp.test($input.val()));
    }

    function changePassCheck(event) {
        $(".pass-input", $employeePrompt).prop("readonly", !$(event.target).prop("checked"));
    }

    function changeRoleSelect(event) {
        var isUsersRole = +$(event.target).val() === usersRoleId;

        $(".pass-check", $employeePrompt).prop("checked", !isUsersRole);
        $(".pass-check", $employeePrompt).trigger("change");
        $(".pass-check", $employeePrompt).prop("disabled", isUsersRole);
    }

    function clickDepsTable(event) {
        var depId = $(event.target).closest("tr").data("dep-id"),
            depName = $(event.target).text() + "",
            titleName;

        if (depName.length > 25) {
            titleName = depName.substr(0, 25) + "...";
        }
        else {
            titleName = depName;
        }

        $(".editDep", $navTabs).removeClass("hide");
        $(".editDep > a", $navTabs).tab("show");
        $(".editDep > a > b", $navTabs).text(titleName);

        $editDepTab.data("dep-id", depId);
        $editDepTab.data("dep-name", depName);

        //clearQuestionPrompt();
        employeesListUpdate(depId);
    }

    function clickAddEmployeeBtn() {
        var depId = $editDepTab.data("dep-id");

        $(".employee-prompt").remove();
        $employeePrompt = $employeePromptTemplate.clone(true);
        $employeePrompt.removeClass("employee-prompt-template").addClass("employee-prompt");

        if (+depId === instpectorsDepId) {
            $(".role-select", $employeePrompt).empty().append("<option value='4'>Инспектор</option>");
        }

        $employeePrompt.modal("show");
        $(".role-select", $employeePrompt).trigger("change");
    }

    function clickEmployeeTable() {
        var $currentRow = $(event.target).closest("tr"),
            employeeId = $currentRow.data("employee-id"),
            firstName = $(".firstname-td", $currentRow).text(),
            lastName = $(".lastname-td", $currentRow).text(),
            roleId = $(".role-td", $currentRow).data("role-id"),
            depId = $editDepTab.data("dep-id");

        $(".employee-prompt").remove();
        $employeePrompt = $employeePromptTemplate.clone(true);
        $employeePrompt.removeClass("employee-prompt-template").addClass("employee-prompt");

        $employeePrompt.data("employee-id", employeeId);
        $(".firstname-input", $employeePrompt).val(firstName);
        $(".lastname-input", $employeePrompt).val(lastName);
        $(".pass-input", $employeePrompt).val("").prop("readonly", true);
        $(".pass-check", $employeePrompt).prop("checked", false);

        if (+depId === instpectorsDepId) {
            $(".role-select", $employeePrompt).empty().append("<option value='4'>Инспектор</option>");
        } else {
            $(".role-select", $employeePrompt).val(roleId);
        }

        $employeePrompt.modal("show");
        $(".role-select", $employeePrompt).trigger("change");
    }

    function clickRemoveEmployeeBtn() {
        var employeeId = $employeePrompt.data("employee-id") + "",
            firstName = $(".firstname-input", $employeePrompt).val(),
            lastName = $(".lastname-input", $employeePrompt).val(),
            depId = $editDepTab.data("dep-id");

        if (employeeId === "-1") {
            return;
        }

        $employeePrompt.modal("hide");
        $removePrompt.modal("show");
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
                    employeesListUpdate(depId);
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function clickSaveEmployeeBtn() {
        var employeeId = $employeePrompt.data("employee-id") + "",
            firstName = $(".firstname-input", $employeePrompt).val(),
            lastName = $(".lastname-input", $employeePrompt).val(),
            password = $(".pass-input", $employeePrompt).val(),
            roleId = $(".role-select", $employeePrompt).val();

        if (!nameExp.test(lastName) || !nameExp.test(firstName)) {
            showError("Не допустимое значения полей ввода");
            return;
        }

        if (employeeId === "-1" && +roleId !== usersRoleId && !$(".pass-check", $employeePrompt).prop("checked")) {
            showError("Пароль обязателен при добаление пользователя");
            return;
        }

        if (employeeId === "-1" || $(".pass-check", $employeePrompt).prop("checked")) {
            if (!passwExp.test(password) && +roleId !== usersRoleId) {
                showError("Пароль должен содержать не менее 6 символов без пробелов");
                return;
            }
        } else {
            password = "";
        }

        saveEmployee(employeeId, firstName, lastName, password, roleId);
    }

    function saveEmployee(employeeId, firstName, lastName, password, roleId) {
        var depId = $editDepTab.data("dep-id");

        $(".save-employee-btn", $employeePrompt).button("loading");

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            cache: false,
            data: {
                queryName: "saveEmployeeByDep",
                depid: depId,
                employeeid: employeeId,
                firstname: firstName,
                lastname: lastName,
                password: password,
                roleid: roleId
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
            employeesListUpdate(depId);
        });
    }

    function employeesListUpdate(depId) {
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
                populateEmployeeTable(result.Data);
            } else {
                showError(result.Error);
            }
        });
    }

    function populateEmployeeTable(data) {
        var $row;

        $(".employees-table tbody", $editDepTab).empty();

        if (data !== null) {
            $(data).each(function (index, el) {
                $row = $("<tr data-employee-id=\"" + el.Id + "\"></tr>").clone();
                $row.append("<td class='lastname-td'>" + el.LastName + "</td>");
                $row.append("<td class='firstname-td'>" + el.FirstName + "</td>");
                $row.append("<td class='role-td' data-role-id=" + el.Role_Id + ">" + roleIdToString(el.Role_Id) + "</td>");
                $(".employees-table tbody", $editDepTab).append($row);
            });
        }
    }

    function depsListUpdate() {

        $.ajax({
            url: "RegularAjax",
            method: "get",
            cache: false,
            data: {
                queryName: "listDeps"
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                populateDepsTable(result.Data);
            } else {
                showError(result.Error);
            }
        });
    }

    function populateDepsTable(data) {
        var $row;

        $(".deps-table tbody", $depsTab).empty();

        if (data !== null) {
            $(data).each(function (index, el) {
                if (el.Id !== superadminsDepId) {
                    $row = $("<tr data-dep-id=\"" + el.Id + "\"></tr>").clone();
                    $row.append("<td>" + el.Name + "</td>");
                    $(".deps-table tbody", $depsTab).append($row);
                }
            });
        }
    }

    function removeFromMemory(event) {
        //$(event.target).remove();
        //$(".modal-backdrop").remove();
        $(this).data("modal", null);
    }

    function roleIdToString(roleId) {
        switch(roleId) {
            case 1:
                return "Администратор отдела";

            case 2: 
                return "Пользователь";

            case 3:
                return "Супер Администратор";

            case 4:
                return "Инспектор";

            default:
                return roleId + "";
        }
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

    function updateSizeTextArea($textArea, time) {
        setTimeout(function () { autosize.update($textArea); }, time);
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());