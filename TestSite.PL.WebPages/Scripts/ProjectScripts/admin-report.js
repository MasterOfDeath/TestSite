(function () {
    var $content = $(".content-admin"),
        $tabContent = $(".tab-content", $content),
        $reportTab = $(".report-tab", $tabContent),
        dateExp = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/;

    $(".datepicker", $reportTab).datepicker({ todayBtn: "linked", language: "ru", autoclose: true });
    $(".report-refresh-btn", $reportTab).click(clickRefreshBtn);
    $(".report-save-btn", $reportTab).click(saveTable);

    function clickRefreshBtn() {
        var dateStart = $(".date-start", $reportTab).val() + "",
            dateEnd = $(".date-end", $reportTab).val() + "";

        if ( !dateExp.test(dateStart) || !dateExp.test(dateEnd)) {
            showError("Дата начала или окончания периода заданы не верно");
            return;
        }

        getReportByDate(dateStart, dateEnd);
    }

    function getReportByDate(dateStart, dateEnd) {

        $.ajax({
            url: "AdminsAjax",
            method: "get",
            data: {
                queryName: "getReportByDate",
                datestart: dateStart,
                dateend: dateEnd
            }
        }).success(function (data) {
            var result = JSON.parse(data, dateTimeReviver);

            if (result.Error === null) {
                populateReportTable(result.Data);
            } else {
                showError(result.Error);
            }
        });
    }

    function populateReportTable(data) {
        var tRows = [],
            $row;

        $(".report-table tbody", $reportTab).empty();

        if (data !== null) {
            $(data).each(function (index, el) {
                $row = $("<tr></tr>").clone();
                $row = $("<tr data-report-id=\"" + el.Id + "\"></tr>").clone();
                $row.append("<td>" + el.Date + "</td>");
                $row.append("<td>" + el.Employee + "</td>");
                $row.append("<td>" + el.Test + "</td>");
                $row.append("<td>" + el.ErrCount + "</td>");
                $row.append("<td>" + el.ErrPercent + "</td>");
                tRows.push($row);
            });

            $(".report-table tbody", $reportTab).append(tRows);
        }
    }

    function saveTable() {
        var dt = new Date(),
            day = dt.getDate(),
            month = dt.getMonth() + 1,
            year = dt.getFullYear(),
            hour = dt.getHours(),
            mins = dt.getMinutes(),
            postfix = dt.getDate() + "." + month + "." + year + "_" + hour + "." + mins;

        $(".report-table", $reportTab).table2excel({
            exclude: ".excludeThisClass",
            name: "Worksheet Name",
            filename: "Протокол-" + postfix //do not include extension
        });
    }

    function dateTimeReviver(key, value) {
        if (key === "Date") {
            return moment(value).utc().format("DD.MM.YYYY HH:mm");
        }
        return value;
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());