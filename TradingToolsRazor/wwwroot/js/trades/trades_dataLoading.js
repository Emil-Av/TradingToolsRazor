/**
 * ******************************
 * Region data loading
 * ******************************
 */

function setHiddenSpansValues(viewModel) {
    // Update the trade data object with new values
    window.tradeData = {
        tradeId: viewModel.tradesVM.currentTrade.id,
        journalId: viewModel.tradesVM.currentTrade.journalId,
        sampleSizeId: viewModel.tradesVM.currentTrade.sampleSizeId,
        reviewId: viewModel.tradesVM.currentSampleSize.review.id
    };
}

function loadViewData(tradeNumber) {
    setMenuValues(tradeNumber);
    setSelectedItemClass();
    loadImages();
    loadReview();
    loadJournal();
    loadTradeData();
}

function loadTradeData() {
    const currentTrade = tradesViewModel.tradesVM.currentTrade;

    $('#SymbolInput').val(currentTrade.symbol);
    $('#StatusInput').val(currentTrade.status);
    $('#TriggerPriceInput').val(currentTrade.triggerPriceInput);
    $('#EntryPriceInput').val(currentTrade.entryPriceInput);
    $('#StopPriceInput').val(currentTrade.stopPriceInput);
    $('#ExitPriceInput').val(currentTrade.exitPriceInput);
    $('#TargetsInput').val(currentTrade.targetsInput);
    $('#PnLInput').val(currentTrade.pnLInput);
    $('#FeeInput').val(currentTrade.feeInput);
}

function loadReview() {
    // Reset all review tab panes
    $('#reviewTabContent .tab-pane').removeClass('show active');
    // Activate the 'First' tab pane
    $('#first').addClass('show active');
    // Reset and activate the 'First' tab button
    $('#reviewTabHeaders .nav-link').removeClass('active').attr('aria-selected', 'false');
    $('#first-tab').addClass('active').attr('aria-selected', 'true');

    const review = tradesViewModel.tradesVM.currentSampleSize.review;

    // Set the values
    $('#first').html(review.first);
    $('#second').html(review.second);
    $('#third').html(review.third);
    $('#forth').html(review.forth);
    $('#summary').html(review.summary);
}

function loadJournal() {
    // Reset all journal tab panes
    $('#journalTabContent .tab-pane').removeClass('show active');
    // Activate the 'Pre' tab pane
    $('#pre').addClass('show active');
    // Reset and activate the 'Pre' tab button
    $('#journalTabHeaders .nav-link').removeClass('active').attr('aria-selected', 'false');
    $('#pre-tab').addClass('active').attr('aria-selected', 'true');

    const journal = tradesViewModel.tradesVM.currentTrade.journal;

    // Set the values
    $('#pre').html(journal.pre);
    $('#during').html(journal.during);
    $('#exit').html(journal.exit);
    $('#post').html(journal.post);
}

function loadImages() {
    $('#imageContainer').empty();
    const screenshots = tradesViewModel.tradesVM.currentTrade.screenshotsUrls;

    let carouselHtml = '<ol class="carousel-indicators">';
    for (let i = 0; i < screenshots.length; i++) {
        if (i === 0) {
            carouselHtml += '<li data-bs-target="#carouselTrades" data-slide-to="' + i + '" class="active"></li>';
        } else {
            carouselHtml += '<li data-bs-target="#carouselTrades" data-slide-to="' + i + '"></li>';
        }
    }
    carouselHtml += '</ol>';

    carouselHtml += '<div class="carousel-inner">';
    for (let i = 0; i < screenshots.length; i++) {
        const imageUrl = screenshots[i];
        if (i === 0) {
            carouselHtml += '<div class="carousel-item active"><img src="' + imageUrl + '" class="d-block w-100" alt="..." /></div>';
        } else {
            carouselHtml += '<div class="carousel-item"><img src="' + imageUrl + '" class="d-block w-100" alt="..." /></div>';
        }
    }
    carouselHtml += '</div>';

    $('#imageContainer').html(carouselHtml);
}
