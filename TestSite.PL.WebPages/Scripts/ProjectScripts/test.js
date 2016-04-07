(function () {
    var $content = $(".content-test"),
        $tabContent = $(".tab-content", $content),
        $navTabs = $(".nav-tabs", $content),
        $checkTestBtn = $(".check-test-btn", ".navbar"),
        $wrongAlert = $(".alert", $content);


    $(document).ready(getRandomTest);
    $checkTestBtn.click(clickCheckTestBtn);

    function clickCheckTestBtn() {
        var result = [],
            checked = false,
            checkedAnswerId = -1,
            exit = false;

        $(".tab-pane", $tabContent).each(function (index, elQ) {
            checked = false;

            $(":radio", elQ).each(function (index, elA) {
                if ($(elA).prop("checked")) {
                    checked = true;
                    checkedAnswerId = $(elA).data("answer-id");
                }
            });

            if (checked === false) {
                showError("Не все ответы отмечены!");
                exit = true;
                return false;
            }

            result.push({ questionId: $(elQ).data("question-id"), answerId: checkedAnswerId });
        });

        // Если экстренно вышли из предыдущего each - выходим из функции
        if (exit === true) {
            return;
        }

        checkMyAnswers(result, $content.data("test-id"));
    }

    function checkMyAnswers(result, testId) {
        $checkTestBtn.button("loading");

        $.ajax({
            url: "UsersAjax",
            method: "post",
            data: {
                queryName: "checkMyAnswers",
                testid: testId,
                answers: JSON.stringify(result)
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                if (result.Data === true) {
                    youWon();
                }
                else {
                    showWrongAlert();
                }
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $checkTestBtn.button("reset");
        });
    }

    function getRandomTest() {
        $.ajax({
            url: "UsersAjax",
            method: "post",
            data: {
                queryName: "getRandomTest"
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                drawTestsTabs(result.Data);
            } else {
                showError(result.Error);
            }
        });
    }

    function drawTestsTabs(test) {
        var $templateTab = $(".tab-template", $content),
            $templateRadio = $(".radio-template", $content),
            $newTab,
            $newRadio;

        $(".test-title", $content).text(test.test.Name);
        $content.data("test-id", test.test.Id);

        $(test.questions).each(function (indexQ, elQ) {
            $newTab = $templateTab.clone();
            $newTab.removeClass("hide").removeClass("tab-template");
            $newTab.data("question-id", elQ.question.Id);
            $newTab.prop("id", "tab-" + elQ.question.Id);

            $(".question-title", $newTab).text(elQ.question.Name);
            $navTabs.append("<li><a data-toggle='tab' href='#tab-" + elQ.question.Id + "'>Вопрос " + (indexQ + 1) + "</a></li>");

            $(elQ.answers).each(function (indexA, elA) {
                $newRadio = $templateRadio.clone();
                $newRadio.removeClass("hide").removeClass("radio-template");
                $(":radio", $newRadio).prop("name", "value-" + elQ.question.Id);
                $(":radio", $newRadio).data("answer-id", elA.Id);
                $("label", $newRadio).append(elA.Name);

                $(".radio-container", $newTab).append($newRadio);
            });

            $tabContent.append($newTab);
        });
    }

    function showWrongAlert() {
        $(".wrong-msg-container", $content).empty().append("<div class='alert alert-danger'>" +
                "<a href='#' class='close' data-dismiss='alert' aria-label='close'>&times;</a>" +
                "Ответ не правильный!</div>");
    }

    function youWon() {
        $(".wrong-msg-container", $content).empty().append("<div class='alert alert-success'>" +
                "Вы правильно решили тест!</div>");
        $tabContent.empty();
        $navTabs.empty();
        $checkTestBtn.unbind("click");
        $(".back-btn", ".navbar").text("Назад")
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());