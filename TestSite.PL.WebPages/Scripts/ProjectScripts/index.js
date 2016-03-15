(function () {
    var $content = $(".content");

    $(".carousel-indicators > li").first().addClass("active");
    $(".carousel-inner > .item").first().addClass("active");

    $(".more-btn", $content).click(function (e) {
        e.preventDefault();
        $(".well .alluserstext", $content).toggleClass("ellipsis");

        $(e.target).text(($(e.target).text() === "More") ? "Less" : "More");
    });

    $(".more-btn", $content).toggleClass("hide", !isElementTextOverflow($(".well .alluserstext", $content)));

    function isElementTextOverflow($element) {
        var $c = $element
           .clone()
           .css({ display: 'inline', width: 'auto', visibility: 'hidden' })
           .appendTo('body'),
            result = false;

        result = $c.width() > $element.width();

        $c.remove();

        return result;
    }
}());