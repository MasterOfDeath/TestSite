(function () {
    var $content = $(".content-login"),
        $depSelector = $(".dep-selector", $content),
        $employeeSelector = $(".employee-selector", $content);

    $depSelector.change(clickDepSelector);

    // Для отрисовки $employeeSelector
    clickDepSelector();

    function clickDepSelector() {
        var depId = $depSelector.val();

        $employeeSelector.prop("disabled", true);

        $.ajax({
            url: "RegularAjax",
            method: "post",
            data: {
                queryName: "clickDepSelector",
                depid: depId
            }
        }).success(function (data) {
            var result = JSON.parse(data);

            if (result.Error === null) {
                $employeeSelector.empty();
                $employeeSelector.append($(result.Data));
            } else {
                showError(result.Error);
            }
        }).always(function () {
            $employeeSelector.prop("disabled", false);
        });
    }

    function showError(str) {
        var $modal = $(".errorModal", $content);

        $(".modal-body", $modal).text(str);
        $modal.modal();
    }
}());