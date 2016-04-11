﻿(function () {
    var $content = $(".content-test"),
        $tabContent = $(".tab-content", $content),
        $navTabs = $(".nav-tabs", $content),
        $checkTestBtn = $(".check-test-btn", ".navbar"),
        $wrongAlert = $(".alert", $content);


    $(document).ready(getMixedTest);
    $checkTestBtn.click(clickCheckTestBtn);

    function clickCheckTestBtn() {
        var result = [],
            answers = [],
            checked = false,
            checkedAnswerId = -1,
            exit = false;

        $(".tab-pane", $tabContent).each(function (index, elQ) {
            checked = false;
            answers = [];

            $(".check-input", elQ).each(function (index, elA) {
                if ($(elA).prop("checked")) {
                    answers.push($(elA).data("answer-id"));
                }
            });

            if (answers.length === 0) {
                showError("Не все ответы отмечены!");
                exit = true;
                return false;
            }

            result.push({ questionId: $(elQ).data("question-id"), answers: answers });
        });

        // Если экстренно вышли из предыдущего each - выходим из функции
        if (exit) { return; }

        checkMyAnswers(result, $content.data("test-id"));
    }

    function checkMyAnswers(results, testId) {
        $checkTestBtn.button("loading");

        $.ajax({
            url: "UsersAjax",
            method: "post",
            data: {
                queryName: "checkMyAnswers",
                testid: testId,
                results: JSON.stringify(results)
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                if (result.Data.correct === true) {
                    youWon();
                }
                else {
                    showWrongAlert(result.Data.wrongQuestions);
                }
                console.log(result.Data);
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $checkTestBtn.button("reset");
        });
    }

    function getMixedTest() {
        var testId = $content.data("test-id") + "";

        $.ajax({
            url: "UsersAjax",
            method: "post",
            data: {
                queryName: "getMixedTest",
                testid: testId
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
            $newTabHeader,
            $newInput,
            $schemaContainer,
            $picture,
            $zoomIn, $zoomOut, $reset;

        $(".test-title", $content).text(test.test.Name);

        $(test.questions).each(function (indexQ, elQ) {
            $newTab = $templateTab.clone();
            $newTab.removeClass("hide").removeClass("tab-template");
            $newTab.data("question-id", elQ.question.Id);
            $newTab.prop("id", "tab-" + elQ.question.Id);
            $newTabHeader = $(".tab-header-template li", $content).clone();
            $("a", $newTabHeader).prop("href", "#tab-" + elQ.question.Id);
            $("a", $newTabHeader).text("Вопроc " + (indexQ + 1));

            $(".question-title", $newTab).text(elQ.question.Name);
            $navTabs.append($newTabHeader);

            if (elQ.question.Type === 1 || elQ.question.Type === 2) {
                $templateInput = $(".radio-template .radio", $content);
            }
            else {
                $templateInput = $(".checkbox-template .checkbox", $content);
            }

            $(elQ.answers).each(function (indexA, elA) {
                $newInput = $templateInput.clone();
                $(".check-input", $newInput).prop("name", "value-" + elQ.question.Id);
                $(".check-input", $newInput).data("answer-id", elA.Id);
                $("label", $newInput).append(elA.Name);

                $(".answers-container", $newTab).append($newInput);
            });

            $tabContent.append($newTab);

            if (elQ.question.Type === 2 || elQ.question.Type === 4) {
                $schemaContainer = $(".schema-container-template", $content).clone();
                $schemaContainer.removeClass("hide").removeClass("schema-container-template");
                $newTab.append($schemaContainer);

                $(".schema", $schemaContainer).attr("src", "/Pages/GetImage.cshtml?queryName=getSchema&questionid=" + elQ.question.Id + "&time=" + new Date().getTime());

                $(".panzoom", $schemaContainer).panzoom({
                    $zoomIn: $(".zoom-in-btn", $schemaContainer),
                    $zoomOut: $(".zoom-out-btn", $schemaContainer),
                    $reset: $(".reset-btn", $schemaContainer)
                });
            }
        });

        $(".nav-tabs li a", $content).first().tab("show");
    }

    function showWrongAlert(wrongQuestions) {
        var wrongText,
            $newWrongAlert = $(".wrong-alert-template").clone();

        wrongText = $.map(wrongQuestions, function (el) {
            return "<ul><li>" + el + "</li></ul>"
        }).join("");

        $newWrongAlert.removeClass("wrong-alert-template").removeClass("hide");
        $(".wrong-questions", $newWrongAlert).append(wrongText);

        $(".wrong-msg-container", $content).empty().append($newWrongAlert);
    }

    function youWon() {
        $(".wrong-msg-container", $content).empty().append("<div class='alert alert-success'>" +
                "Вы правильно решили тест!</div>");
        $checkTestBtn.unbind("click").addClass("hide");
        $(".back-btn", ".navbar").text("Закрыть");
        $(".check-input", $tabContent).click(function () { return false; });
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());