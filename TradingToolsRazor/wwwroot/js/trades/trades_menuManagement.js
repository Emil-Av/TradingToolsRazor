/**
 * ******************************
 * Region menu management
 * ******************************
 */

function setMenuValues(displayedTradeNumber) {
    const viewModel = tradesViewModel.tradesVM;

    // Menu Buttons
    const numberSampleSizes = viewModel.numberSampleSizes !== -1 ? viewModel.numberSampleSizes : '-';
    const numberTrades = viewModel.tradesInSampleSize !== 0 ? viewModel.tradesInSampleSize : viewModel.tradesInTimeFrame;

    // Set the SampleSize menu
    $('#spanSampleSize').text(viewModel.currentSampleSizeNumber);
    $('#dropdownBtnSampleSize').empty();

    let sampleSizesHtml = '';
    for (let i = numberSampleSizes; i > 0; i--) {
        sampleSizesHtml += '<a class="dropdown-item" role="button">' + i + '</a>';
    }
    $('#dropdownBtnSampleSize').html(sampleSizesHtml);

    // Update trade navigation display
    if (shouldShowLastTrade === true) {
        $('#tradeNumberInput').val(numberTrades);
        tradeIndex = numberTrades - 1;
    } else if (numberTrades < displayedTradeNumber) {
        $('#tradeNumberInput').val(numberTrades);
        tradeIndex = numberTrades - 1;
    } else {
        $('#tradeNumberInput').val(displayedTradeNumber);
        tradeIndex = displayedTradeNumber - 1;
    }
    
    // Update total trades display
    updateTotalTradesDisplay(numberTrades);

    setTimeFrameMenu(
        viewModel.availableTimeframes,
        getTimeFrameMapping(),
        viewModel.currentSampleSize.timeFrame
    );

    // Menu card header
    currentCardMenuId = CARD_MENU.JOURNAL;
    $('#cardMenuTradeData').trigger('click');
}

function setSelectedItemClass(buttonSelector) {
    if (buttonSelector) {
        $(buttonSelector + ' a').each(function () {
            if ($(this).text() === $(menuButtons[buttonSelector]).text()) {
                $(this).addClass('bg-gray-400');
            } else {
                $(this).removeClass('bg-gray-400');
            }
        });
    } else {
        for (const key in menuButtons) {
            $(key + ' a').each(function () {
                if ($(this).text() === $(menuButtons[key]).text()) {
                    $(this).addClass('bg-gray-400');
                } else {
                    $(this).removeClass('bg-gray-400');
                }
            });
        }
    }
}
