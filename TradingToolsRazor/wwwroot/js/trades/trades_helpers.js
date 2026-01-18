/**
 * ******************************
 * Region helper functions
 * ******************************
 * 
 * Note: The following functions are defined in site.js and used here:
 * - getTradeData()
 * - getJournalData()
 * - validateNumberInputs()
 * - setTimeFrameMenu()
 */

function tradesLoadRequest(sampleSizeId) {
    // Get values from the select elements
    const strategy = parseInt($('#selectStrategy').val());
    const tradeType = parseInt($('#selectTradeType').val());
    const status = parseInt($('#selectStatus').val());
    const timeFrame = parseInt($('#selectTimeFrame').val());

    // Construct the request object matching TradesLoadRequestModel
    const requestModel = {
        Strategy: strategy,
        TradeType: tradeType,
        Status: status,
        TimeFrame: timeFrame,
        SampleSizeId: sampleSizeId
    };

    return requestModel;
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

function getTimeFrameMapping() {
    return {
        "M5": "5M",
        "M10": "10M",
        "M15": "15M",
        "M30": "30M",
        "H1": "1H",
        "H2": "2H",
        "H4": "4H",
        "D": "D"
    };
}

function getStrategy() {
    const strategyValue = $('#selectStrategy').val();
    return strategyValue ? parseInt(strategyValue) : null;
}

function getReviewData() {
    let review = {};

    $('#cardBody [data-review-data]').each(function () {
        var bindProperty = $(this).data('review-data');
        var content = $(this).html().trim();
        review[bindProperty] = content;
    });

    if (typeof window.tradeData !== 'undefined') {
        review['Id'] = window.tradeData.reviewId;
    }

    return review;
}

function getResearchData() {
    let researchData = {};

    if (getStrategy() === STRATEGY.SRS) {
        // Collect all research data fields to ensure all properties are included in the update
        $('#cardBody [data-research-data]').each(function () {
            var bindProperty = $(this).data('research-data');
            var value = $(this).val();

            // Handle CandleType as integer enum value
            if (bindProperty === 'CandleType') {
                researchData[bindProperty] = value !== null && value !== undefined && value !== '' ? parseInt(value) : null;
            }
            // Handle boolean values
            else if (value === "true") {
                researchData[bindProperty] = true;
            } else if (value === "false") {
                researchData[bindProperty] = false;
            } else if (value === "" || value === null || value === undefined) {
                researchData[bindProperty] = null;
            } else {
                researchData[bindProperty] = value;
            }
        });
        
        // Ensure all required properties are present even if not in the form
        // This prevents partial updates where some properties might be missing
        if (typeof window.tradeData !== 'undefined') {
            // Include the ID from window.tradeData to ensure proper update
            researchData['Id'] = window.tradeData.tradeId;
        }
    }

    return researchData;
}
