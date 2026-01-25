/**
 * ******************************
 * Region trade navigation
 * ******************************
 * Handles prev/next trade navigation functionality
 */

// Toggles to the next trade
function showNextTrade(index) {
    displayTradeData(index, false);
}

// Toggles to the previous trade
function showPrevTrade(index) {
    displayTradeData(index, false);
}

// Loads the trade at the specified index and updates the UI
function displayTradeData(indexToShow, isUserInput) {
    // Buttons 'prev' or 'next'
    if (!isUserInput && (indexToShow == -1 || indexToShow == 1)) {
        tradeIndex += indexToShow;
    }
    // User input of the trade to be shown
    else {
        tradeIndex = indexToShow;
    }

    if (isUserInput && tradeIndex >= totalTradesInSampleSize) {
        displayToastrErrorWhenSwitchingTrades('Trade ' + (tradeIndex + 1) + ' doesn\'t exist.');
        return;
    }
    else if (isUserInput && tradeIndex < 0) {
        displayToastrErrorWhenSwitchingTrades('Trade number can\'t be smaller than 1.');
        return;
    }
    else if (!isUserInput && tradeIndex >= totalTradesInSampleSize) {
        displayToastrInfoWhenSwitchingTrades('The last trade is being displayed');
        return;
    }
    else if (!isUserInput && tradeIndex < 0) {
        displayToastrInfoWhenSwitchingTrades('The first trade is being displayed');
        return;
    }

    lastTradeIndex = tradeIndex;
    $('#tradeNumberInput').val(tradeIndex + 1);

    // Load the trade from local array (no API call)
    loadTradeByIndex(tradeIndex + 1); // tradeIndex is 0-based, but function expects 1-based
}

function displayToastrErrorWhenSwitchingTrades(message) {
    toastr.error(message);
    tradeIndex = lastTradeIndex;
}

function displayToastrInfoWhenSwitchingTrades(message) {
    toastr.info(message);
    tradeIndex = lastTradeIndex;
}

// Load a specific trade by its number (1-based index) from local array
function loadTradeByIndex(tradeNumber) {
    // Convert 1-based to 0-based index
    const index = tradeNumber - 1;
    
    // Check if we have trades loaded locally
    if (allTrades && allTrades.length > 0 && index >= 0 && index < allTrades.length) {
        const trade = allTrades[index];
        
        // Update window.tradeData with the new trade (all properties PascalCase)
        window.tradeData = {
            tradeId: trade.Id,
            journalId: trade.JournalId,
            screenshotsUrls: trade.ScreenshotsUrls,
            sampleSizeId: trade.SampleSizeId,
            reviewId: window.tradeData.reviewId // Keep the same review
        };
        
        // Load the trade data into the UI
        loadTradeDataLocally(trade);
        
        if (getStrategy() === STRATEGY.SRS) {
            loadResearchDataLocally(trade);
        }
        
        // Load images (property is PascalCase)
        loadImagesLocally(trade.ScreenshotsUrls);
        
        // Load journal if available
        if (trade.Journal) {
            loadJournalLocally(trade.Journal);
        }
    } else {
        // Fallback to API call if trades not loaded locally (shouldn't happen normally)
        console.warn('Trades not loaded locally, falling back to API call');
    }
}

// Load trade data into form fields from local object
function loadTradeDataLocally(trade) {
    // Basic Info tab (all properties PascalCase from C# serialization)
    $('#InputDate').val(formatDateForInput(trade.Date));
    $('#InputSymbol').val(trade.Symbol || '');
    $('#InputDirection').val(trade.Direction !== null && trade.Direction !== undefined ? trade.Direction : '');
    $('#InputAmount').val(trade.Amount || 0);
    $('#InputStatus').val(trade.Status !== null && trade.Status !== undefined ? trade.Status : '');
    $('#InputOutcome').val(trade.Outcome !== null && trade.Outcome !== undefined ? trade.Outcome : '');
    $('#InputTradeRating').val(trade.TradeRating !== null && trade.TradeRating !== undefined ? trade.TradeRating : '');
    
    // Prices tab (all properties PascalCase)
    $('#InputTriggerPrice').val(trade.TriggerPrice || 0);
    $('#InputEntryPrice').val(trade.EntryPrice || 0);
    $('#InputStopPrice').val(trade.StopPrice || 0);
    $('#InputExitPrice').val(trade.ExitPrice || 0);
    $('#InputMaxPrice').val(trade.MaxPrice || 0);
    
    // Results tab (all properties PascalCase)
    $('#InputPnL').val(trade.PnL || 0);
    $('#InputFee').val(trade.Fee || 0);
}

// Load research data for SRS trades
function loadResearchDataLocally(trade) {
    // Load SRS research data fields (all properties PascalCase)
    if (trade.CandleType !== null && trade.CandleType !== undefined) {
        $('#InputCandleType').val(trade.CandleType);
    }
    
    if (trade.IsInOverNightRange !== null && trade.IsInOverNightRange !== undefined) {
        $('#InputIsInOverNightRange').val(trade.IsInOverNightRange.toString());
    }
    
    if (trade.IsFlippedTheSwitch !== null && trade.IsFlippedTheSwitch !== undefined) {
        $('#InputFlippedTheSwitch').val(trade.IsFlippedTheSwitch.toString());
    }
}

// Load journal data into tabs from local object
function loadJournalLocally(journal) {
    // Journal properties are PascalCase
    $('#pre').html(journal.Pre || 'Double click to add text');
    $('#during').html(journal.During || 'Double click to add text');
    $('#exit').html(journal.Exit || 'Double click to add text');
    $('#post').html(journal.Post || 'Double click to add text');
}

// Load images into carousel from local array
function loadImagesLocally(screenshotsUrls) {
    if (!screenshotsUrls || screenshotsUrls.length === 0) {
        $('#imageContainer').html('<p>No screenshots available</p>');
        return;
    }

    let carouselHtml = '<div class="carousel-indicators">';
    for (let i = 0; i < screenshotsUrls.length; i++) {
        if (i === 0) {
            carouselHtml += '<button type="button" data-bs-target="#carouselTrades" data-bs-slide-to="' + i + '" class="active" aria-current="true" aria-label="Slide ' + (i + 1) + '"></button>';
        } else {
            carouselHtml += '<button type="button" data-bs-target="#carouselTrades" data-bs-slide-to="' + i + '" aria-label="Slide ' + (i + 1) + '"></button>';
        }
    }
    carouselHtml += '</div>';

    carouselHtml += '<div class="carousel-inner">';
    for (let i = 0; i < screenshotsUrls.length; i++) {
        if (i === 0) {
            carouselHtml += '<div class="carousel-item active">';
        } else {
            carouselHtml += '<div class="carousel-item">';
        }
        carouselHtml += '<img src="' + screenshotsUrls[i] + '" class="d-block w-100" alt="Trade screenshot ' + (i + 1) + '">';
        carouselHtml += '</div>';
    }
    carouselHtml += '</div>';

    $('#imageContainer').html(carouselHtml);
}

// Update the total trades count display
function updateTotalTradesDisplay(totalTrades) {
    totalTradesInSampleSize = totalTrades;
    $('#tradesInSampleSize').val('/' + totalTrades);
}

// Initialize trade index from current trade number
function initializeTradeIndex(currentTradeNumber) {
    tradeIndex = currentTradeNumber - 1; // Convert to 0-based index
    lastTradeIndex = tradeIndex;
}
