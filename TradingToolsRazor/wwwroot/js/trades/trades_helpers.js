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
