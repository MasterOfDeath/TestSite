(function () {
    var $content = $(".content-admin"),
        $depsSelectCont = $(".deps-select-container", $content),
        $tabContent = $(".tab-content", $content),
        $removePrompt = $(".remove-prompt", $tabContent),
        $reportTab = $(".report-tab", $tabContent),
        dateExp = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/;

    $(".datepicker", $reportTab).datepicker({ todayBtn: "linked", language: "ru", autoclose: true });
    $(".report-refresh-btn", $reportTab).click(clickRefreshBtn);
    $(".report-save-btn", $reportTab).click(clickSaveBtn);
    $(".deps-select", $depsSelectCont).change(changeDepsSelect);
    $(".report-table", $reportTab).on("click", ".trash-btn", clickTrashBtn);

    function clickRefreshBtn() {
        var requestOwnerId = $content.data("user-id"),
            dateStart = $(".date-start", $reportTab).val() + "",
            dateEnd = $(".date-end", $reportTab).val() + "",
            depId = $(".deps-select", $depsSelectCont).val(),
            emplOrder = $(".employee-sort", $reportTab).prop("checked");;

        if (depId === undefined) {
            depId = -1;
        }

        if ( !dateExp.test(dateStart) || !dateExp.test(dateEnd)) {
            showError("Дата начала или окончания периода заданы не верно");
            return;
        }

        getReportByDep(dateStart, dateEnd, requestOwnerId, depId, emplOrder);
    }

    function clickSaveBtn(event) {
        var requestOwnerId = $content.data("user-id"),
            dateStart = $(".date-start", $reportTab).val() + "",
            dateEnd = $(".date-end", $reportTab).val() + "",
            depId = $(".deps-select", $depsSelectCont).val(),
            emplOrder = $(".employee-sort", $reportTab).prop("checked"),
            spinner;

        if (depId === undefined) {
            depId = -1;
        }

        if (!dateExp.test(dateStart) || !dateExp.test(dateEnd)) {
            showError("Дата начала или окончания периода заданы не верно");
            return;
        }

        spinner = new Spinner({
            lines: 11,
            length: 3,
            width: 2,
            radius: 6
        }).spin(event.target);

        $(".report-save-btn .save-icon", $reportTab).addClass("hide-element");
        $(".report-save-btn .spinner", $reportTab).addClass("show-element");
        $(".report-save-btn", $reportTab).prop("disabled", true);

        $.fileDownload("GetFile", {
            data: {
                queryName: "saveDataToFileByDep",
                requestownerid: requestOwnerId,
                datestart: dateStart,
                dateend: dateEnd,
                depid: depId,
                emplorder: emplOrder
            }
        }).fail(function () {
            showError("Данные отсутствуют");
        }).always(function () {
            spinner.stop();
            $(".report-save-btn .save-icon", $reportTab).removeClass("hide-element");
            $(".report-save-btn", $reportTab).prop("disabled", false);
        });
    }

    function changeDepsSelect() {
        $(".report-table tbody", $reportTab).empty();
    }

    function clickTrashBtn(event) {
        var $currentRow = $(event.target).closest("tr"),
            reportId = $currentRow.data("report-id");

        $removePrompt.modal("show");
        $(".modal-title", $removePrompt).text("Удаление");
        $(".modal-body", $removePrompt).empty().append("<p><b>Вы действительно хотите удалить запись?</b></p>");
        $(".modal-body", $removePrompt).append("<table class='table'><tbody></tbody></table>");
        $(".modal-body tbody", $removePrompt).append($currentRow.clone());
        $(".modal-body tbody tr .trash-btn", $removePrompt).remove();
        $(".remove-prompt-btn", $removePrompt).click(function (event) {
            var $thisBtn = $(event.target);

            $thisBtn.button("loading");

            $.ajax({
                url: "AdminsAjax",
                method: "post",
                cache: false,
                data: {
                    queryName: "removeReport",
                    reportid: reportId
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $removePrompt.modal("hide");
                    $currentRow.remove();
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function getReportByDep(dateStart, dateEnd, requestOwnerId, depId, emplOrder) {
        var spinner = new Spinner({
            lines: 11,
            length: 16,
            width: 7,
            radius: 23
        }).spin($(".report-table", $reportTab).get(0));

        $.ajax({
            url: "UsersAjax",
            method: "get",
            cache: false,
            data: {
                queryName: "getReportByDep",
                requestownerid: requestOwnerId,
                datestart: dateStart,
                dateend: dateEnd,
                depid: depId,
                emplorder: emplOrder
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error !== null) {
                showError(result.Error);
            }

            populateReportTable(result.Data);
        }).always(function () {
            spinner.stop();
        });
    }

    function populateReportTable(data) {
        var $trashSpan = $(".trash-template span", $reportTab),
            $row;

        $(".report-table tbody", $reportTab).empty();

        if (data !== null) {
            $(data).each(function (index, el) {
                $row = $("<tr data-report-id='" + el.Id + "' class='" + el.Mark + "'></tr>").clone();
                $row.append("<td>" + el.Date + "</td>");
                $row.append("<td>" + el.Employee + "</td>");
                $row.append("<td>" + el.Test + "</td>");
                $row.append("<td>" + el.ErrCount + "</td>");
                $row.append("<td>" + translateMark(el.Mark) + "</td>");
                if ($trashSpan !== undefined) {
                    $("td", $row).last().append($trashSpan.clone());
                }
                $(".report-table tbody", $reportTab).append($row);
            });
        }
    }

    function translateMark(mark) {
        if (mark === "perfect") {
            return "Отлично";
        } else if (mark === "good") {
            return "Удавлет.";
        } else {
            return "Не удавл.";
        }
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());