(function () {
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
        $removePrompt = $(".remove-prompt", $tabContent),
        $testNameInput = $(".name-input", $namePrompt),
        $rowForAnswer = $(".row-for-answer", $questionPrompt),
        $saveQuestionBtn = $(".save-question-btn", $questionPrompt),
        currentAnswers = [];
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
        
    function clickAddTestBtn(event) {
        $namePrompt.modal();
        $(".modal-title", $namePrompt).text("Название теста");
        $(".modal-body :text", $namePrompt).val("");
        $(".save-name-btn", $namePrompt).unbind("click").bind("click", {testId: -1}, clickSaveTest);
    }

    function clickAddQuestionBtn() {
        currentAnswers = [];
        $questionPrompt.data("question-id", -1);
        $(".modal-title", $questionPrompt).text("Вопрос");
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

        $questionPrompt.data("question-id", questionId);
        getQuestionAndAnswers(questionId);
    }

    function clickAddAnswerBtn() {
        addAnswer(-1, "", false);
    }

    function clickRemoveAnswerBtn(event) {
        $(event.target).closest(".radio").remove();
    }

    function clickSaveQuestionBtn(event) {
        var questionId = $questionPrompt.data("question-id"),
            text = $(".question-textarea", $questionPrompt).val(),
            testId = $(".editTest-tab", $tabContent).data("test-id"),
            answers = [],
            copyCurrentAnswers = currentAnswers.slice(0),
            hasCorrectAnswer = false;

        if (!isValidName(text)) {
            showError("Недопустимое значние");
            return;
        }

        $(".answers-container > .inline-radio", $questionPrompt).each(function (index, el) {
            if (isValidName($(":text", $(el)).val())) {
                answers.push({
                    answerId: $(el).data("answer-id"),
                    text: $(":text", $(el)).val(),
                    correct: $(":radio", $(el)).prop("checked")
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

        saveQuestionAndAnswers(questionId, testId, text, copyCurrentAnswers);
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
            questionId = $questionPrompt.data("question-id");

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

    function testsListUpdate() {
        $.ajax({
            url: "AdminsAjax",
            method: "post",
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
            method: "post",
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

    function saveQuestionAndAnswers(questionId, testId, text, answers) {
        var $saveQuestionBtn = $(".save-question-btn", $questionPrompt);

        $saveQuestionBtn.button("loading");

        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: {
                queryName: "saveQuestionAndAnswers",
                questionid: questionId,
                testid: testId,
                text: text,
                answers: JSON.stringify(answers)
            }
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
        $newRowForAnswer.removeClass("row-for-answer").removeClass("hide");
        $(".remove-answer-btn", $newRowForAnswer).click(clickRemoveAnswerBtn);

        $newRowForAnswer.data("answer-id", answerId);
        $(":text", $newRowForAnswer).val(text);
        if (correct === true) {
            $(":radio", $newRowForAnswer).prop("checked", true);
        }
    }

    function clearQuestionPrompt() {
        $(".answers-container", $questionPrompt).empty();
        $(".question-textarea", $questionPrompt).val("");
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