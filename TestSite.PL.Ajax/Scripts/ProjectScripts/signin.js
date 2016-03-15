(function () {
    var $content = (".content-login"),
        $nameInput = $(".name-input", $content),
        $usernameInput = $(".username-input", $content),
        $passwordInput = $(".password-input", $content),
        nameExp = /[^\w\- \.]+|[ ]{2,}|[\-\.]{2,}|^[\-\.]+$|^[ \-\.]| $/,
        usernameExp = /[^\w]+|_{2,}|^_|_$|^(.)\1+$/,
        passwExp = /^(?=.{6,}$)([^\s]*[a-zA-Z0-9][^\s]*)+$/;

    $nameInput.change(changeNameInput);
    $nameInput.keyup(changeNameInput);

    $usernameInput.change(changeUserNameInput);
    $usernameInput.keyup(changeUserNameInput);

    $passwordInput.change(changePasswInput);
    $passwordInput.keyup(changePasswInput);

    // Click on "Show SignUp" link
    $(".show-signup", $content).click(function () {
        $(".signin-box", $content).addClass("hide");
        $(".signup-box", $content).removeClass("hide");
    });

    // Click on "Show SignIn" link
    $(".show-signin", $content).click(function () {
        $(".signin-box", $content).removeClass("hide");
        $(".signup-box", $content).addClass("hide");
    });

    function changeNameInput(event) {
        var $input = $(event.target),
            str = $input.val() + "";

        toggleInputError($input, !isValidName(str));
    }

    function changeUserNameInput(event) {
        var $input = $(event.target),
            str = $input.val() + "";

        toggleInputError($input, !isValidUserName(str));
    }

    function changePasswInput(event) {
        var $input = $(event.target),
            str = $input.val() + "";

        toggleInputError($input, str.search(passwExp) === -1);
    }

    function toggleInputError($el, show) {
        var $container = $el.parents(".has-feedback");

        if (show) {
            $container.removeClass("has-success").addClass("has-error");
            $(".badIcon", $container).removeClass("hide");
            $(".goodIcon", $container).addClass("hide");
        } else {
            $container.removeClass("has-error").addClass("has-success");
            $(".badIcon", $container).addClass("hide");
            $(".goodIcon", $container).removeClass("hide");
        }
    }

    function isValidName(str) {
        return (str.length >= 0 && (str.search(nameExp) === -1))
    }

    function isValidUserName(str) {
        return (str.length >= 6 && (str.search(usernameExp) === -1))
    }

    function isValidPassw(str) {
        return (str.length >= 6 && (str.search(passwExp) === -1))
    }
}());