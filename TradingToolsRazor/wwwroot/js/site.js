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

function getJournalData() {
    var journal = {};

    $('#cardBody [data-journal-data]').each(function () {
        var bindProperty = $(this).data('journal-data');
        var content = $(this).html().trim();
        
        if (content === "" || content === "Double click to add text") {
            journal[bindProperty] = null;
        }
        else {
            journal[bindProperty] = content;
        }
    });

    if (typeof window.tradeData !== 'undefined') {
        journal['Id'] = window.tradeData.journalId;
    }

    return journal;
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
    
    // Relevant for Trades page when updating trade trade data
    if (typeof window.tradeData !== 'undefined') {
        tradeData['Id'] = window.tradeData.tradeId;
        tradeData['journalId'] = window.tradeData.journalId;
        tradeData['screenshotsUrls'] = window.tradeData.screenshotsUrls;
        tradeData['sampleSizeId'] = window.tradeData.sampleSizeId;
    }

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

// Map display text to exact enum identifiers used by GetStatisticsModel
// Enums:
// ETimeFrame: M5, M10, M15, M30, H1, H2, H4, D
// EStrategy: FirstBarPullback, Cradle, CandleBracketing
// ETradeType: Trade, Research
function toEnumName(menu, selectedValue) {
    const menuValue = selectedValue.trim();

    if (menu === 'timeFrame') {
        const map = {
            '5M': 0,
            '10M': 1,
            '15M': 2,
            '30M': 3,
            '1H': 4,
            '2H': 5,
            '4H': 6,
            'D': 7
        };
        return map[menuValue];
    }

    if (menu === 'strategy') {
        const map = {
            'First Bar Pullback': 0,
            'Cradle': 1,
            'Candle Bracketing': 2,
            'SRS': 3
        };
        return map[menuValue] || menuValue.replace(/\s+/g, '');
    }

    if (menu === 'tradeType') {
        const map = {
            'Trade': 1,
            'Research': 2
        };
        return map[menuValue] || menuValue;
    }

    if (menu === 'time') {
        // TimeOnly? expects "HH:mm"
        return menuValue;
    }

    return menuValue;
}

