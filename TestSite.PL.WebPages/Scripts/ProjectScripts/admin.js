(function () {
    var $content = $(".content-admin"),
        $navTabs = $(".nav-tabs", $content),
        $tabContent = $(".tab-content", $content),
        $addTestBtn = $(".addTestBtn", $tabContent),
        $addQuestionBtn = $(".addQuestionBtn", $tabContent),
        $testsTable = $(".tests-table", $tabContent),
        $questionsTable = $(".questions-table", $tabContent),
        $namePrompt = $(".name-prompt", $tabContent),
        $questionPrompt = $(".question-prompt", $tabContent),
        $testNameInput = $(".name-input", $namePrompt),
        $rowForAnswer = $(".row-for-answer", $questionPrompt),
        testNameExp = /^\S.+\S$/;

    $addTestBtn.click(clickAddTestBtn);
    $addQuestionBtn.click(clickAddQuestionBtn);
    $testsTable.on("click", "tr", clickTestsTable);
    $(".add-answer-btn", $questionPrompt).click(clickAddAnswerBtn);
        
    function clickAddTestBtn(event) {
        $namePrompt.modal();
        $(".modal-title", $namePrompt).text("Название теста");
        $(".save-name-btn", $namePrompt).click(clickSaveTest);
    }

    function clickAddQuestionBtn() {
        $questionPrompt.modal();
        $(".modal-title", $questionPrompt).text("Вопрос");
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
                testname: testName
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                testsListUpdate();
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

        questionsListUpdate(testId);
    }

    function clickAddAnswerBtn() {
        var $newRowForAnswer = $rowForAnswer.clone(),
            $answersContainer = $(".answers-container", $questionPrompt);

        $answersContainer.append($newRowForAnswer);
        $newRowForAnswer.removeClass("row-for-answer").removeClass("hide");
        $(".remove-answer-btn", $newRowForAnswer).click(clickRemoveAnswerBtn);
    }

    function clickRemoveAnswerBtn(event) {
        $(event.target).closest(".radio").remove();
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
        }).always(function () {
            
        });
    }

    function questionsListUpdate(testId) {
        $.ajax({
            url: "AdminsAjax",
            method: "post",
            data: {
                queryName: "ListQuestionsByTestId",
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
        }).always(function () {

        });
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