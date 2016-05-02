(function () {
    var $content = $(".content-admin"),
        $tabContent = $(".tab-content", $content),
        $testPreview = $(".test-preview", $tabContent),
        $testPreviewTabContent = $(".tab-content", $testPreview),
        $navTabs = $(".nav-tabs", $testPreview);

    $(".testPreviewBtn", $tabContent).click(clickTestPreviewBtn);
    
    function clickTestPreviewBtn(event) {
        clearTestPreview();
        getTestForPreview();
        $testPreview.modal("show");
    }

    function getTestForPreview() {
        var testId = $(".editTest-tab", $tabContent).data("test-id") + "";

        $.ajax({
            url: "AdminsAjax",
            method: "get",
            cache: false,
            data: {
                queryName: "getTestForPreview",
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

    function drawTestsTabs(data) {
        var $templateTab = $(".tab-template", $testPreview),
            $templateTabHeader = $(".tab-header-template li", $testPreview),
            $templateInput,
            $newTab,
            $newTabHeader,
            $newInput,
            $schemaContainer,
            $picture,
            $zoomIn, $zoomOut, $reset;

        $(".test-title", $testPreview).text(data.test.Name);
        $testPreview.data("test-id", data.test.Id);

        $(data.questions).each(function (indexQ, elQ) {
            $newTab = $templateTab.clone();
            $newTab.removeClass("hide").removeClass("tab-template");
            $newTab.data("question-id", elQ.question.Id);
            $newTab.prop("id", "tab-" + elQ.question.Id);
            $newTabHeader = $templateTabHeader.clone();
            $("a", $newTabHeader).prop("href", "#tab-" + elQ.question.Id);
            $("a", $newTabHeader).text("Вопроc " + (indexQ + 1));

            $(".question-title", $newTab).text(elQ.question.Name);
            $navTabs.append($newTabHeader);

            if (elQ.question.Type === 1 || elQ.question.Type === 2) {
                $templateInput = $(".radio-template .radio", $testPreview);
            }
            else {
                $templateInput = $(".checkbox-template .checkbox", $testPreview);
            }

            $(elQ.answers).each(function (indexA, elA) {
                $newInput = $templateInput.clone();
                $newInput.click(function () { return false; });
                $(".check-input", $newInput).prop("name", "value-" + elQ.question.Id);
                $(".check-input", $newInput).data("answer-id", elA.Id);
                $("label", $newInput).append(elA.Name);

                if (elA.Correct) {
                    $(".check-input", $newInput).prop("checked", "checked");
                }

                $(".answers-container", $newTab).append($newInput);
            });

            $testPreviewTabContent.append($newTab);

            if (elQ.question.Type === 2 || elQ.question.Type === 4) {
                $schemaContainer = $(".schema-container-template", $testPreview).clone();
                $schemaContainer.removeClass("hide").removeClass("schema-container-template");
                $newTab.append($schemaContainer);

                $(".schema", $schemaContainer).attr("src", "/Pages/GetFile.cshtml?queryName=getSchema&questionid=" + elQ.question.Id + "&time=" + new Date().getTime());
                $schemaContainer.imagefit();

                $(".panzoom", $schemaContainer).panzoom({
                    $zoomIn: $(".zoom-in-btn", $schemaContainer),
                    $zoomOut: $(".zoom-out-btn", $schemaContainer),
                    $reset: $(".reset-btn", $schemaContainer)
                });
            }

                      
        });

        $(".nav-tabs li a", $testPreview).first().tab("show");
    }

    function clearTestPreview() {
        $navTabs.empty();
        $testPreviewTabContent.empty();
        $testPreview.data("test-id", "-1");
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());