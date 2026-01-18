/**
 * ******************************
 * Region API calls
 * ******************************
 */

function loadSampleSize(sampleSizeId) {
    window.location.href = `/trades?handler=LoadSampleSize&sampleSizeId=${sampleSizeId}`;
}

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
                        }
                    } else if (data.error) {
                        toastr.error(data.error);
                    }
                })
                .catch(error => handleApiError('Error deleting trade', error));
        }
    });
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
