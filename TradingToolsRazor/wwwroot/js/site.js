
// Left bar (menu 'Trades'): Hide the menu if it's clicked outside of it
$(document).on('click', function (event) {
    var menu = $('#dropdownBtnTrades');
    var target = $(event.target);
    var isClickInsideMenu = target.closest('#dropdownBtnTrades').length > 0;

    if (!isClickInsideMenu && menu.hasClass('show')) {
        // If the click is outside of the menu and the menu is open, collapse it
        menu.removeClass('show');
    }
});


// Toggles the visibility of the loading indicator and the main content of a page
var loadingIndicator = $('#loadingIndicator');
var content = $('#myContent');
function showLoadingIndicator() {
    content.removeClass('showMyContent').addClass('hideMyContent');
    loadingIndicator.css('display', 'block');
}

function hideLoadingIndicator() {
    content.removeClass('hideMyContent').addClass('showMyContent');
    loadingIndicator.css('display', 'none');
}

// Gets the data from the elements which have data-trade-data attribute. Used in multiple views.
function getTradeData() {
    var tradeData = {};
    $('#cardBody [data-trade-data]').each(function () {
        var bindProperty = $(this).data('trade-data');
        if ($(this).val() === "") {
            tradeData[bindProperty] = null;
        }
        else {
            tradeData[bindProperty] = $(this).val();
        }
    });
    tradeData['Id'] = $('#spanTradeIdInput').val();

    return tradeData;
}

function validateNumberInputs() {

    let isValid = true;
    $(".number-input").each(function () {
        const value = $(this).val();
        if (value !== "" && !$.isNumeric(value)) {
            $(this).addClass("is-invalid");
            isValid = false;
        }
        else if ($(this).hasClass("is-invalid")) {
            $(this).removeClass("is-invalid");
        }
    })

    if (!isValid) {
        toastr.error("The marked fields must be either empty or contain a number.<br>For decimal separator use a point.");
    }

    return isValid;
}

/**
 * Explanation of the Regular Expression
    ^: Asserts the start of the string.
    -?: Allows an optional negative sign.
    \d+: Matches one or more digits.
    (\,\d+)?: Matches an optional decimal point followed by one or more digits.
    $: Asserts the end of the string.
 */
function isNumeric(value) {
    const regex = /^-?\d+(\,\d+)?$/;
    return regex.test(value);
}

function setTimeFrameMenu(timeframes, timeFrameMapping, currentTimeFrame) {
    $('#dropdownBtnTimeFrame').empty();
    let availableTimeFrames = '';
    let j = 0;
    for (let i = timeframes.length - 1; i >= 0; i--) {
        availableTimeFrames += '<a class="dropdown-item" role="button">' + timeFrameMapping[timeframes[j]] + '</a>';
        j++;
    }
    $('#dropdownBtnTimeFrame').html(availableTimeFrames);

    // Corrected the issue here by removing the invalid empty brackets
    $('#spanTimeFrame').text(timeFrameMapping[currentTimeFrame]);
}

