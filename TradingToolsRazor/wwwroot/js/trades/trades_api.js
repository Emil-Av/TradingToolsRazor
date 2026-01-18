/**
 * ******************************
 * Region API calls
 * ******************************
 */

function updateReview() {
    const review = getReviewData();

    sendPostRequest('/trades?handler=UpdateReview', review)
        .then(data => {
            handleApiResponse(data);
            // No need to update window.tradeData for review as it's sample-size level, not trade level
        })
        .catch(error => handleApiError('Error updating review', error));
}

function updateResearchData() {
    const researchData = getResearchData();
    const tradeData = getTradeData();
    const mergedData = { ...researchData, ...tradeData };

    const requestData = {
        Data: JSON.stringify(mergedData),
        Strategy: getStrategy()
    };

    sendPostRequest('/trades?handler=UpdateResearchData', requestData)
        .then(data => {
            handleApiResponse(data);
            if (data.success) {
                // Update local trade data in allTrades array
                updateLocalTradeData(mergedData);
            }
        })
        .catch(error => handleApiError('Error updating research data', error));
}

function updateTradeData() {
    const tradeData = getTradeData();

    sendPostRequest('/trades?handler=UpdateTradeData', tradeData)
        .then(data => {
            handleApiResponse(data);
            if (data.success) {
                // Update local trade data in allTrades array
                updateLocalTradeData(tradeData);
            }
        })
        .catch(error => handleApiError('Error updating trade data', error));
}

function updateJournal() {
    const journal = getJournalData();

    sendPostRequest('/trades?handler=updateJournal ', journal)
        .then(data => {
            handleApiResponse(data);
            if (data.success) {
                // Update the journal in the current trade in allTrades array
                if (allTrades && allTrades.length > 0 && tradeIndex >= 0 && tradeIndex < allTrades.length) {
                    if (!allTrades[tradeIndex].Journal) {
                        allTrades[tradeIndex].Journal = {};
                    }
                    allTrades[tradeIndex].Journal = { ...allTrades[tradeIndex].Journal, ...journal };
                }
            }
        })
        .catch(error => handleApiError('Error updating journal', error));
}

function loadTrade(status, timeFrame, strategy, tradeType, sampleSize, trade, showLastTrade, loadLastSampleSize, statusChanged) {
    const tradeParams = {
        StatusFromView: status,
        TimeFrameFromView: timeFrame,
        StrategyFromView: strategy,
        TradeTypeFromView: tradeType,
        SampleSizeNumberFromView: sampleSize,
        TradeNumberFromView: trade,
        ShowLastTradeFromView: showLastTrade,
        LoadLastSampleSizeFromView: loadLastSampleSize,
        StatusChangedFromView: statusChanged
    };

    sendPostRequest('/trades?handler=loadtrade', { tradeParams })
        .then(data => {
            if (data.error !== undefined) {
                toastr.error(data.error);
                clickedMenu.text(clickedMenuValue);
                return;
            } else if (data.info !== undefined) {
                toastr.info(data.info);
                clickedMenu.text(clickedMenuValue);
                return;
            }

            tradesViewModel = data;
            
            // Update local trades array with new data
            if (data.tradesVM && data.tradesVM.allTradesInSampleSize) {
                allTrades = data.tradesVM.allTradesInSampleSize;
                totalTradesInSampleSize = allTrades.length;
                console.log('Loaded ' + allTrades.length + ' trades after menu change');
            }
            
            setHiddenSpansValues(tradesViewModel);
            loadViewData(trade);
        })
        .catch(error => {
            handleApiError('Error loading trade', error);
            clickedMenu.text(clickedMenuValue);
        });
}

/**
 * Updates the local trade data in the allTrades array and window.tradeData
 * @param {object} updatedData - The updated trade data from the form
 */
function updateLocalTradeData(updatedData) {
    if (!allTrades || allTrades.length === 0 || tradeIndex < 0 || tradeIndex >= allTrades.length) {
        console.warn('Cannot update local trade data: invalid state');
        return;
    }

    // Update the trade in the allTrades array
    const currentTrade = allTrades[tradeIndex];
    
    // Merge updated data into the current trade object
    // This preserves all existing properties and only updates the ones that changed
    Object.keys(updatedData).forEach(key => {
        // Skip properties that shouldn't be updated from the form
        if (key !== 'Id' && key !== 'JournalId' && key !== 'SampleSizeId' && 
            key !== 'ScreenshotsUrls' && key !== 'Journal') {
            // Convert string values to appropriate types
            let value = updatedData[key];
            
            // Handle null or empty string
            if (value === '' || value === 'null') {
                value = null;
            }
            // Handle boolean strings
            else if (value === 'true') {
                value = true;
            }
            else if (value === 'false') {
                value = false;
            }
            // Convert numeric strings to numbers
            else if (value !== null && !isNaN(value) && value !== '' && typeof value === 'string') {
                value = parseFloat(value);
            }
            
            currentTrade[key] = value;
        }
    });

    console.log('Updated local trade data at index ' + tradeIndex);
}

function sendPostRequest(url, data) {
    const token = document.getElementById('__RequestVerificationToken')?.value;

    return fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...(token && { 'RequestVerificationToken': token })
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json());
}

function handleApiResponse(data) {
    if (data.success !== undefined) {
        toastr.success(data.success);
    } else {
        toastr.error(data.error);
    }
}

function handleApiError(message, error) {
    console.error(message + ':', error);
    toastr.error(message + '. See console for details.');
}
