/**
 * ******************************
 * Region event handlers
 * ******************************
 */

function handleTabChange(event) {
    if (isEditorShown) {
        saveEditorText();
    }
    currentTabSelector = '#' + $(event.target).attr('aria-controls');
}

function handleUpdateClick() {
    if (!validateNumberInputs()) {
        toastr.error('Please enter valid numeric values for trade data fields.');
        return;
    }

    if (currentCardMenuId === CARD_MENU.TRADING_DATA) {
        updateTradeData();
    }
    else if (currentCardMenuId === CARD_MENU.RESEARCH) {
        updateResearchData();
    }
}

function handleDeleteClick() {

}

function handleCancelClick() {
    $('#summernote').summernote('destroy');
    $(currentTabSelector).removeClass('d-none');
    $('#cardBody').addClass('card-body');
    isEditorShown = false;
    toggleFooterButtons();
}

function handleCardMenuClick() {
    if (isEditorShown) {
        saveEditorText();
    }

    currentCardMenuId = $(this).attr('id');
    if (currentCardMenuId === CARD_MENU.TRADING_DATA) {
        showTradingDataContent();
    } else if (currentCardMenuId === CARD_MENU.JOURNAL) {
        showJournalContent();
    } else if (currentCardMenuId === CARD_MENU.RESEARCH) {
        showResearchContent();
    } else {
        showReviewContent();
    }

    updateCardMenuHeader($(this));
}

// Menu button "Next" click event handler
function handleNextClick() {
    showNextTrade(1);
}

// Menu button "Previous" click event handler
function handlePrevClick() {
    showPrevTrade(-1);
}

// User enters trade number and presses enter
function handleTradeNumberInput(event) {
    if (event.which === 13) {
        const userInput = Number(event.target.value);
        if (Number.isInteger(userInput) && userInput > 0) {
            tradeIndex = userInput - 1;
            displayTradeData(tradeIndex, true);
        } else {
            toastr.error("Please enter a valid trade number.");
        }
    }
}

// Event handler when the left or the right arrow is pressed. Displays the trade accordingly.
function handleKeyboardNavigation(event) {
    // Left arrow key pressed
    if (event.which === 37) {
        showPrevTrade(-1);
    }
    // Right arrow key pressed
    else if (event.which === 39) {
        showNextTrade(1);
    }
}

// Handle changes to menu select dropdowns
function handleMenuSelectChange(selectId, selectedValue, selectedText) {
    // You can implement specific logic for each select element here
    switch(selectId) {
        case 'selectStrategy':
            console.log('Strategy changed:', selectedText);
            // Add strategy-specific logic here
            break;
        case 'selectTradeType':
            console.log('Trade Type changed:', selectedText);
            // Add trade type-specific logic here
            break;
        case 'selectStatus':
            console.log('Status changed:', selectedText);
            // Add status-specific logic here
            break;
        case 'selectTimeFrame':
            loadTimeFrame()
            // Add time frame-specific logic here
            break;
        case 'selectSampleSize':
            loadSampleSize(selectedValue);
            break;
        default:
            console.log('Unknown select changed:', selectId);
    }
}
