$(function () {
    /**
   * ******************************
   * Region input styles starts
   * ******************************
   */
    $('.editableInput').on('focus', function () {
        $(this).removeClass('bg-gray-500');
        $(this).addClass('bg-gradient-light');
    });

    $('.editableInput').on('blur', function () {
        $(this).removeClass('bg-gradient-light');
        $(this).addClass('bg-gray-500');
    });

    $('.editableInput').on('keypress', function (event) {
        // Check if the Enter key is pressed (key code 13)
        if (event.which === 13) {
            $(this).blur();
        }
    });

    $('#formCalculator').on('keypress', function (event) {
        if (event.which === 13) {
            event.preventDefault();
        }
    });

    // Remove focus from the inputs if enter is pressed
    $('#entryInput, #stopInput').on('keypress', function (event) {
        if (event.which === 13) {
            $(this).blur();
        }
    });

    /**
  * ******************************
  * Region input styles ends
  * ******************************
  */
});