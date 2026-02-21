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

function handleScreenshotFileChange(event) {
    const files = event.target.files;
    
    if (!files || files.length === 0) {
        return;
    }

    // Validate file types (only images)
    const validImageTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/bmp'];
    const invalidFiles = [];
    
    for (let i = 0; i < files.length; i++) {
        if (!validImageTypes.includes(files[i].type)) {
            invalidFiles.push(files[i].name);
        }
    }
    
    if (invalidFiles.length > 0) {
        toastr.error('Only image files are allowed. Invalid files: ' + invalidFiles.join(', '));
        event.target.value = ''; // Clear the input
        return;
    }

    // Create FormData for multipart/form-data
    const formData = new FormData();
    
    // Add all selected files
    for (let i = 0; i < files.length; i++) {
        formData.append('files', files[i]);
    }
    
    // Add trade ID from window.tradeData
    if (typeof window.tradeData !== 'undefined' && window.tradeData.tradeId) {
        formData.append('tradeId', window.tradeData.tradeId);
    } else {
        toastr.error('Unable to identify current trade.');
        event.target.value = ''; // Clear the input
        return;
    }

    uploadScreenshotsAsync(formData, event);
}

function handleDeleteClick() {
    Swal.fire({
        title: "Are you sure?",
        text: "All data incl. screenshots will be gone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then((result) => {
        if (result.isConfirmed) {
            deleteTrade();
        }
    });
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
    switch (selectId) {
        case 'selectStrategy':
            loadStrategy(selectedValue)
            break;
        case 'selectSampleSizeType':
            loadSampleSizeType(selectedValue);
            break;
        // Add sample size type-specific logic here
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
