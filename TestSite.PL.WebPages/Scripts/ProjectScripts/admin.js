﻿(function () {
    var $content = $(".content-admin"),
        $navTabs = $(".nav-tabs", $content),
        $tabContent = $(".tab-content", $content),
        $addTestBtn = $(".addTestBtn", $tabContent),
        $editTestBtn = $(".editTestBtn", $tabContent),
        $removeTestBtn = $(".removeTestBtn", $tabContent),
        $addQuestionBtn = $(".addQuestionBtn", $tabContent),
        $testsTable = $(".tests-table", $tabContent),
        $questionsTable = $(".questions-table", $tabContent),
        $namePrompt = $(".name-prompt", $tabContent),
        $questionPrompt = $(".question-prompt", $tabContent),
        $testPreview = $(".test-preview", $tabContent),
        $removePrompt = $(".remove-prompt", $tabContent),
        $testNameInput = $(".name-input", $namePrompt),
        $rowForAnswer = $(".radio-for-answer", $questionPrompt),
        $saveQuestionBtn = $(".save-question-btn", $questionPrompt),
        currentAnswers = [],
        testNameExp = /^\S.+\S$/;

    $addTestBtn.click(clickAddTestBtn);
    $editTestBtn.click(clickEditTestBtn);
    $removeTestBtn.click(clickRemoveTestBtn);
    $addQuestionBtn.click(clickAddQuestionBtn);
    $saveQuestionBtn.click(clickSaveQuestionBtn);
    $testsTable.on("click", "tr", clickTestsTable);
    $questionsTable.on("click", "tr", clickQuestionsTable);
    $(".remove-question-btn", $questionPrompt).click(clickRemoveQuestionBtn);
    $(".add-answer-btn", $questionPrompt).click(clickAddAnswerBtn);
    $(".type-select", $questionPrompt).change(changeTypeSelect);
        
    function clickAddTestBtn(event) {
        $namePrompt.modal();
        $(".modal-title", $namePrompt).text("Название теста");
        $(".modal-body :text", $namePrompt).val("");
        $(".save-name-btn", $namePrompt).unbind("click").bind("click", {testId: -1}, clickSaveTest);
    }

    function clickAddQuestionBtn() {
        currentAnswers = [];
        clearQuestionPrompt();
        $(".modal-title", $questionPrompt).text("Вопрос");
        setQuestionType("1");
        $questionPrompt.modal("show");
    }

    function clickSaveTest(event) {
        var $thisBtn = $(event.target),
            testName = $testNameInput.val();

        if (!isValidName(testName)) {
            showError("Недопустимое имя");
            return;
        }

        $thisBtn.button("loading");

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: {
                queryName: "clickSaveTestBtn",
                testid: event.data.testId,
                testname: testName
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                testsListUpdate();
                $(".editTest", $navTabs).addClass("hide");
                $(".listTests > a", $navTabs).tab("show");
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $namePrompt.modal("hide");
            $testNameInput.val("");
            $thisBtn.button("reset");
        });
    }

    function clickTestsTable(event) {
        var testId = $(event.target).closest("tr").data("id"),
            testName = $(event.target).html();

        $(".editTest", $navTabs).removeClass("hide");
        $(".editTest > a", $navTabs).tab("show");

        $(".editTest-tab h2", $tabContent).text(testName);
        $(".editTest-tab", $tabContent).data("test-id", testId);

        clearQuestionPrompt();
        questionsListUpdate(testId);
    }

    function clickQuestionsTable(event) {
        var questionId = $(event.target).closest("tr").data("id");

        getQuestionAndAnswers(questionId);
    }

    function clickAddAnswerBtn() {
        addAnswer(-1, "", false);
    }

    function clickRemoveAnswerBtn(event) {
        $(event.target).closest("." + getAnswerTypeInText()).remove();
    }

    function clickSaveQuestionBtn(event) {
        var questionId = $questionPrompt.data("question-id") + "",
            questionType = $questionPrompt.data("question-type") + "",
            text = $(".question-textarea", $questionPrompt).val(),
            testId = $(".editTest-tab", $tabContent).data("test-id"),
            answers = [],
            copyCurrentAnswers = currentAnswers.slice(0),
            hasCorrectAnswer = false,
            image;

        if (!isValidName(text)) {
            showError("Недопустимое значние");
            return;
        }

        $(".answers-container > ." + getAnswerTypeInText(), $questionPrompt).each(function (index, el) {
            if (isValidName($(":text", $(el)).val())) {
                answers.push({
                    answerId: $(el).data("answer-id"),
                    text: $(":text", $(el)).val(),
                    correct: $(".check-input", $(el)).prop("checked")
                });
            }
        });

        $(answers).each(function (index, el) {
            if (el.answerId === -1) {
                el.action = "insert";
                copyCurrentAnswers.push(el);
            }
        });

        $(copyCurrentAnswers).each(function (index1, el1) {
            if (el1.action === undefined) {
                $(answers).each(function (index2, el2) {
                    if (el1.answerId === el2.answerId) {
                        if (el1.text === el2.text && el1.correct === el2.correct) {
                            copyCurrentAnswers[index1].action = "none";
                        }
                        else {
                            copyCurrentAnswers[index1] = $.extend({}, el2);
                            copyCurrentAnswers[index1].action = "update";
                        }
                    }
                });
            }
        });

        copyCurrentAnswers = $.grep(copyCurrentAnswers, function (el, index) {
            if (el.action === undefined) {
                copyCurrentAnswers[index].action = "delete";
            }

            return el.action !== "none";
        });

        $(answers).each(function (index, el) {
            if (el.correct) {
                hasCorrectAnswer = true;
            }
        });

        if (!hasCorrectAnswer) {
            showError("Хотя бы один ответ должен быть правильным");
            return;
        }

        if ((questionType === "2" || questionType === "4")) {
            if ($(":file", $questionPrompt).val() !== "") {
                image = $(":file", $questionPrompt).get(0).files[0];
            }
            else {
                if (questionId === "-1") {
                    showError("Схема не выбрана");
                    return;
                }

                image = null;
            }
        }
        else {
            image = null;
        }

        saveQuestionAndAnswers(questionId, testId, text, copyCurrentAnswers, questionType, image);
    }

    function clickEditTestBtn(event) {
        var textTest = $(".editTest-tab h2", $tabContent).text(),
            testId = $(".editTest-tab", $tabContent).data("test-id");

        $namePrompt.modal("show");
        $(".modal-body :text", $namePrompt).val(textTest);
        $(".save-name-btn", $namePrompt).unbind("click").bind("click", { testId: testId }, clickSaveTest);
    }

    function clickRemoveTestBtn(event) {
        var textTest = $(".editTest-tab h2", $tabContent).text(),
            testId = $(".editTest-tab", $tabContent).data("test-id");

        $removePrompt.modal("show");
        $(".modal-title", $removePrompt).text("Удаление теста");
        $(".modal-body p", $removePrompt).text(textTest);
        $(".remove-prompt-btn", $removePrompt).click(function (event) {
            var $thisBtn = $(event.target);

            $thisBtn.button("loading");

            $.ajax({
                url: "AdminsAjax",
                method: "post",
                data: {
                    queryName: "removeTest",
                    testid: testId
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $removePrompt.modal("hide");
                    testsListUpdate();
                    $(".editTest", $navTabs).addClass("hide");
                    $(".listTests > a", $navTabs).tab("show");
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function clickRemoveQuestionBtn(event) {
        var textQuestion = $(".question-textarea", $questionPrompt).val(),
            questionId = $questionPrompt.data("question-id") + "";

        if (questionId === "-1") {
            showError("Вопрос не найден");
            return;
        }

        $removePrompt.modal("show");
        $(".modal-title", $removePrompt).text("Удаление вопроса");
        $(".modal-body p", $removePrompt).text(textQuestion);

        $(".remove-prompt-btn", $removePrompt).click(function (event) {
            var $thisBtn = $(event.target);

            $thisBtn.button("loading");

            $.ajax({
                url: "AdminsAjax",
                method: "post",
                data: {
                    queryName: "removeQuestion",
                    questionid: questionId
                }
            }).success(function (data) {
                var result = JSON.parse(data);

                if (result.Error === null) {
                    $removePrompt.modal("hide");
                    $questionPrompt.modal("hide");
                    questionsListUpdate($(".editTest-tab", $tabContent).data("test-id"));
                } else {
                    showError(result.Error);
                }
            }).always(function () {
                $thisBtn.button("reset");
            });
        });
    }

    function changeTypeSelect(event) {
        setQuestionType($(event.target).val());
    }

    function testsListUpdate() {
        $.ajax({
            url: "AdminsAjax",
            method: "get",
            data: {
                queryName: "listAllTests"
            }
        }).success(function (data) {
            var result = JSON.parse(data),
                $tbody = $("<tbody>");

            if (result.Error === null) {
                $(result.Data).each(function (index, el) {
                    $tbody.append("<tr data-id=\"" + el.Id +"\"><td>" + el.Name + "</td></tr>");
                });

                $testsTable.empty().append($tbody);
            } else {
                showError(result.Error);
            }
        });
    }

    function questionsListUpdate(testId) {
        $.ajax({
            url: "AdminsAjax",
            method: "get",
            data: {
                queryName: "listQuestionsByTestId",
                testid: testId
            }
        }).success(function (data) {
            var result = JSON.parse(data),
                $tbody = $("<tbody>");

            if (result.Error === null) {
                $(result.Data).each(function (index, el) {
                    $tbody.append("<tr data-id=\"" + el.Id + "\"><td>" + el.Name + "</td></tr>");
                });

                $questionsTable.empty().append($tbody);
            } else {
                showError(result.Error);
            }
        });
    }

    function saveQuestionAndAnswers(questionId, testId, text, answers, questionType, image) {
        var $saveQuestionBtn = $(".save-question-btn", $questionPrompt),
            formData = new FormData();

        if (image === null) {
            formData.append("image", null);
        }
        else {
            formData.append("image", image);
        }

        formData.append("queryName", "saveQuestionAndAnswers");
        formData.append("questionid", questionId);
        formData.append("testid", testId);
        formData.append("text", text);
        formData.append("questiontype", questionType);
        formData.append("answers", JSON.stringify(answers));

        $saveQuestionBtn.button("loading");

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: formData,
            processData: false,
            contentType: false
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                questionsListUpdate(testId);
                $questionPrompt.modal("hide");
                clearQuestionPrompt();
            }
            else {
                showError(result.Error);
            }
        }).always(function () {
            $saveQuestionBtn.button("reset");
        });
    }

    function getQuestionAndAnswers(questionId) {
        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: {
                queryName: "getQuestionAndAnswers",
                questionid: questionId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                clearQuestionPrompt();
                $questionPrompt.data("question-id", questionId);
                setQuestionType(result.Data.questionType);
                $(".type-select", $questionPrompt).val(result.Data.questionType).prop("disabled", "disabled");
                $questionPrompt.modal("show");
                $(".modal-title", $questionPrompt).text("Вопрос");
                $(".question-textarea", $questionPrompt).val(result.Data.text);
                currentAnswers = [];
                $(result.Data.answers).each(function (index, el) {
                    addAnswer(el.Id, el.Name, el.Correct);
                    currentAnswers.push({answerId: el.Id, text: el.Name, correct: el.Correct});
                });
            }
            else {
                showError(result.Error);
            }
        });
    }

    function addAnswer(answerId, text, correct) {
        var $newRowForAnswer = $rowForAnswer.clone(),
            $answersContainer = $(".answers-container", $questionPrompt);

        $answersContainer.append($newRowForAnswer);
        $newRowForAnswer.removeClass(getAnswerTypeInText() + "-for-answer").removeClass("hide");
        $(".remove-answer-btn", $newRowForAnswer).click(clickRemoveAnswerBtn);

        $newRowForAnswer.data("answer-id", answerId);
        $(":text", $newRowForAnswer).val(text);
        if (correct === true) {
            $(".check-input", $newRowForAnswer).prop("checked", true);
        }
    }

    function setQuestionType(questionType) {
        questionType = questionType + "";
        $questionPrompt.data("question-type", questionType);

        if (questionType === "2" || questionType === "4") {
            $(".file-upload-panel", $questionPrompt).removeClass("hide");
        }
        else {
            $(".file-upload-panel", $questionPrompt).addClass("hide");
            $(".file-upload-panel :file", $questionPrompt).val("");
        }

        if (questionType === "1" || questionType === "2") {
            // "radio";
            $(".answers-container .checkbox").addClass("radio").removeClass("checkbox");
            $(".answers-container .check-input", $questionPrompt).replaceWith("<input type='radio' class='check-input' name='value1' value='value1' />");
        }
        else {
            // "checkbox";
            $(".answers-container .radio").addClass("checkbox").removeClass("radio");
            $(".answers-container .check-input", $questionPrompt).replaceWith("<input type='checkbox' class='check-input' value='' />");
        }

        $rowForAnswer = $("." + getAnswerTypeInText() + "-for-answer", $questionPrompt);
    }

    function getAnswerTypeInText() {
        var questionType = $questionPrompt.data("question-type") + "";

        if (questionType === "1" || questionType === "2") {
            return "radio";
        }
        else {
            return "checkbox";
        }
    }

    function clearQuestionPrompt() {
        $questionPrompt.data("question-id", -1).data("question-type", -1);
        $(".answers-container", $questionPrompt).empty();
        $(".question-textarea", $questionPrompt).val("");
        $(".file-upload-panel", $questionPrompt).addClass("hide");
        $(".file-upload-panel :file", $questionPrompt).val(null);
        $(".type-select", $questionPrompt).val("1").removeAttr("disabled");
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }

    function isValidName(str) {
        return (str.length > 0 && (str.search(testNameExp) !== -1))
    }
}());