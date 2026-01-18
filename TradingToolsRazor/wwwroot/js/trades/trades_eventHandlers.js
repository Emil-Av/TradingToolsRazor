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

function handleMenuButtonClick(buttonSelector, clickedElement) {
    // Save the old value. If there is no trade in the DB for the selected trade, the menu's old value should be displayed.
    clickedMenu = $(menuButtons[buttonSelector]);
    clickedMenuValue = $(menuButtons[buttonSelector]).text();

    // If the time frame, the strategy or the sample size has changed, then the latest trade must always be displayed. 
    // Used in setMenuValues()
    if (buttonSelector !== '#dropdownBtnTrade') {
        shouldShowLastTrade = true;
    } else {
        shouldShowLastTrade = false;
    }

    if (buttonSelector === '#dropdownBtnTimeFrame') {
        shouldLoadLastSampleSize = true;
    } else if (buttonSelector === '#dropdownBtnStatus') {
        hasStatusChanged = true;
    } else {
        shouldLoadLastSampleSize = false;
        hasStatusChanged = false;
    }

    // Set the new value
    const selectedValue = clickedElement.text();
    $(menuButtons[buttonSelector]).text(selectedValue);

    loadTrade(
        $('#spanStatus').text(),
        $('#spanTimeFrame').text(),
        $('#spanStrategy').text(),
        $('#spanTradeType').text(),
        $('#spanSampleSize').text(),
        $('#spanTrade').text(),
        shouldShowLastTrade,
        shouldLoadLastSampleSize,
        hasStatusChanged
    );
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
            const deleteTradeRequest = {
                Id: window.tradeData.tradeId,
                Strategy: getStrategy()
            };

            sendPostRequest('/trades?handler=DeleteTrade', deleteTradeRequest)
                .then(data => {
                    if (data.success) {
                        toastr.success(data.success);

                        // Remove the trade from local array
                        if (allTrades && allTrades.length > 0) {
                            allTrades.splice(tradeIndex, 1);
                            totalTradesInSampleSize = allTrades.length;
                            updateTotalTradesDisplay(totalTradesInSampleSize);
                            
                            // Navigate to previous trade or first trade if we deleted the first one
                            if (allTrades.length > 0) {
                                const newIndex = Math.max(0, Math.min(tradeIndex, allTrades.length - 1));
                                tradeIndex = newIndex;
                                loadTradeByIndex(newIndex + 1);
                            } else {
                                // No more trades, reload page to show "No Trades Yet"
                                window.location.reload();
                            }
                        } else {
                            // Fallback: reload via API
                            loadTrade(
                                $('#selectStatus option:selected').text().replace('Status: ', ''),
                                $('#selectTimeFrame option:selected').text().replace('Time Frame: ', ''),
                                $('#selectStrategy option:selected').text().replace('Strategy: ', ''),
                                $('#selectTradeType option:selected').text().replace('Type: ', ''),
                                $('#selectSampleSize').val(),
                                1,
                                true, // shouldShowLastTrade
                                false, // shouldLoadLastSampleSize
                                false // hasStatusChanged
                            );
                        }
                    } else if (data.error) {
                        toastr.error(data.error);
                    }
                })
                .catch(error => handleApiError('Error deleting trade', error));
        }
    });
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
