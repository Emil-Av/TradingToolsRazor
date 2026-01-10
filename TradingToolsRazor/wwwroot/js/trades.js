// Summernote sometimes opens with the cursor in the middle, then that causes errors. Check Review
$(function () {

    /**
    * ******************************
    * Region global variables and constants
    * ******************************
    */

    // Menu state tracking - When a new trade has to be loaded, one of the buttons has to be clicked (either TimeFrame, Strategy..). 
    // In case no trade exists for the selection, set the last value. Used in loadTrade()
    let clickedMenu;
    let clickedMenuValue;
    let shouldShowLastTrade;
    let shouldLoadLastSampleSize = false;
    let hasStatusChanged = false;
    let tradesViewModel;
    let currentTabSelector = '#pre'; // Always the start value
    let isEditorShown = false;
    let currentCardMenuId = 'itemTradingData';

    // Card menu constants
    const CARD_MENU = {
        TRADING_DATA: 'itemTradingData',
        JOURNAL: 'itemJournal',
        REVIEW: 'itemReview',
        RESEARCH: 'itemResearch'
    };

    // Strategy enum constants
    const STRATEGY = {
        FIRST_BAR_PULLBACK: 0,
        CRADLE: 1,
        CANDLE_BRACKETING: 2,
        SRS: 3
    };

    // Create key, value array: key is the button menu, value is the span element. 
    // The span element is the selected value from the dropdown menu.
    const menuButtons = {
        '#dropdownBtnStatus': '#spanStatus',
        '#dropdownBtnTimeFrame': '#spanTimeFrame',
        '#dropdownBtnStrategy': '#spanStrategy',
        '#dropdownBtnTradeType': '#spanTradeType',
        '#dropdownBtnSampleSize': '#spanSampleSize',
        '#dropdownBtnTrade': '#spanTrade'
    };

    /**
    * ******************************
    * Region initialization
    * ******************************
    */

    function initialize() {
        initializeEventHandlers();
        initializeMenuButtons();
    }

    function initializeEventHandlers() {
        // Summernote editor events
        $('#tabContentJournal').on('dblclick', openEditor);
        $('#tabContentReview').on('dblclick', openEditor);
        $('button[data-bs-toggle="tab"]').on('shown.bs.tab', handleTabChange);

        // Button click events
        $('#btnUpdate').on('click', handleUpdateClick);
        $('#btnDelete').on('click', handleDeleteClick);
        $('#btnEdit').on('click', openEditor);
        $('#btnSave').on('click', saveEditorText);
        $('#btnCancel').on('click', handleCancelClick);

        // Card menu events
        $('#headerMenu').on('click', '.dropdown-item', handleCardMenuClick);
    }

    function initializeMenuButtons() {
        for (const key in menuButtons) {
            (function (buttonSelector) {
                setSelectedItemClass(buttonSelector);
                $(buttonSelector).on('click', '.dropdown-item', function () {
                    handleMenuButtonClick(buttonSelector, $(this));
                });
            })(key);
        }
    }

    /**
    * ******************************
    * Region API calls
    * ******************************
    */

    function updateReview() {
        const review = getReviewData();

        sendPostRequest('/trades?handler=UpdateReview', review)
            .then(handleApiResponse)
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
            .then(handleApiResponse)
            .catch(error => handleApiError('Error updating research data', error));
    }

    function updateTradeData() {
        const tradeData = getTradeData();

        sendPostRequest('/trades?handler=UpdateTradeData', tradeData)
            .then(handleApiResponse)
            .catch(error => handleApiError('Error updating trade data', error));
    }

    function updateJournal() {
        const journal = getJournalData();

        sendPostRequest('/trades?handler=updateJournal ', journal)
            .then(handleApiResponse)
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

        sendPostRequest('/trades/loadtrade', { tradeParams })
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
                setHiddenSpansValues(tradesViewModel);
                loadViewData(trade);
            })
            .catch(error => {
                handleApiError('Error loading trade', error);
                clickedMenu.text(clickedMenuValue);
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

                            // Reload the trades or update the UI
                            loadTrade(
                                $('#spanStatus').text(),
                                $('#spanTimeFrame').text(),
                                $('#spanStrategy').text(),
                                $('#spanTradeType').text(),
                                $('#spanSampleSize').text(),
                                $('#spanTrade').text(),
                                true, // shouldShowLastTrade
                                false, // shouldLoadLastSampleSize
                                true // hasStatusChanged
                            );
                        } else if (data.error) {
                            toastr.error(data.error);
                        }
                    })
                    .catch(error => handleApiError('Error deleting trade', error));
            }
        });
    }

    /**
    * ******************************
    * Region summernote editor
    * ******************************
    */

    function openEditor() {
        $('#cardBody').removeClass('card-body');
        toggleFooterButtons();

        // Hide the tabContent of the journal and show the summernote instead
        // Get the text from the tabContent
        const currentTabContent = $(currentTabSelector).html();
        // Hide the tabContent
        $(currentTabSelector).addClass('d-none');
        // Set the text into the editor
        $('#summernote').summernote('code', currentTabContent);
        $('#summernote').summernote('justifyLeft');
        // Display the editor
        isEditorShown = true;
    }

    function saveEditorText() {
        $('#cardBody').addClass('card-body');
        isEditorShown = false;

        // Save the text from the editor
        const editorText = $($('#summernote').summernote('code')).text().trim();
        // Save the text from the tab
        const oldTabContent = $(currentTabSelector).text().trim();
        // Set the content of the tab to the value of the editor
        $(currentTabSelector).html($('#summernote').summernote('code'));
        // Show the tab content
        $(currentTabSelector).removeClass('d-none');
        // Close the editor
        $('#summernote').summernote('code', '');
        $('#summernote').summernote('destroy');
        toggleFooterButtons();

        // If a change has been made, save it
        if (editorText !== oldTabContent) {
            if (currentCardMenuId === CARD_MENU.JOURNAL) {
                updateJournal();
            } else {
                updateReview();
            }
        }
    }

    function toggleFooterButtons() {
        if ($('#btnEdit').hasClass('d-none')) {
            $('#btnEdit').removeClass('d-none');
            $('.editorOnBtns').addClass('d-none');
        } else {
            $('#btnEdit').addClass('d-none');
            $('.editorOnBtns').removeClass('d-none');
        }
    }

    /**
    * ******************************
    * Region card menu content display
    * ******************************
    */

    function showTradingDataContent() {
        $('#tradeDataTabContent').removeClass('d-none');
        $('#journalTabHeaders').addClass('d-none');
        $('#journalTabContent').addClass('d-none');
        $('#reviewTabHeaders').addClass('d-none');
        $('#reviewTabContent').addClass('d-none');
        $('#researchTabContent').addClass('d-none');
        $('#btnEdit').addClass('d-none');
        $('#btnUpdate').removeClass('d-none');
        $('#btnDelete').removeClass('d-none');
    }

    function showJournalContent() {
        currentTabSelector = '#pre';
        // Ensure the Pre tab is active when showing journal content
        $('#pre').addClass('show active').siblings('.tab-pane').removeClass('show active');
        $('#pre-tab').addClass('active').attr('aria-selected', 'true')
            .siblings('.nav-link').removeClass('active').attr('aria-selected', 'false');

        $('#journalTabHeaders').removeClass('d-none');
        $('#journalTabContent').removeClass('d-none');
        $('#reviewTabHeaders').addClass('d-none');
        $('#reviewTabContent').addClass('d-none');
        $('#tradeDataTabContent').addClass('d-none');
        $('#researchTabContent').addClass('d-none');
        $('#btnEdit').removeClass('d-none');
        $('#btnUpdate').addClass('d-none');
        $('#btnDelete').addClass('d-none');
    }

    function showResearchContent() {
        $('#researchTabContent').removeClass('d-none');
        $('#tradeDataTabContent').addClass('d-none');
        $('#journalTabHeaders').addClass('d-none');
        $('#journalTabContent').addClass('d-none');
        $('#reviewTabHeaders').addClass('d-none');
        $('#reviewTabContent').addClass('d-none');
        $('#btnEdit').addClass('d-none');
        $('#btnUpdate').removeClass('d-none');
        $('#btnDelete').removeClass('d-none');
    }

    function showReviewContent() {
        currentTabSelector = '#first';
        $('#reviewTabHeaders').removeClass('d-none');
        $('#reviewTabContent').removeClass('d-none');
        $('#journalTabHeaders').addClass('d-none');
        $('#journalTabContent').addClass('d-none');
        $('#tradeDataTabContent').addClass('d-none');
        $('#researchTabContent').addClass('d-none');
        $('#btnEdit').removeClass('d-none');
        $('#btnUpdate').addClass('d-none');
        $('#btnDelete').addClass('d-none');
    }

    function updateCardMenuHeader(clickedElement) {
        // Set the text of the card menu
        if (currentCardMenuId !== $('#currentMenu').text()) {
            let menuText = '';
            if (currentCardMenuId === CARD_MENU.TRADING_DATA) {
                menuText = 'Trade Data';
            } else if (currentCardMenuId === CARD_MENU.JOURNAL) {
                menuText = 'Journal';
            }
            else if (currentCardMenuId === CARD_MENU.RESEARCH) {
                menuText = 'Research';
            }
            else {
                menuText = 'Review';
            }
            $('#currentMenu').text(menuText);
            clickedElement.addClass('bg-gray-400');

            // Remove the bg color of the last selected item
            $('#headerMenu a').each(function () {
                // if a dropdown item has the bg-gray-400 class and the text of the current item being iterated 
                // is different than the text in the #currentMenu, than that was the previously selected option
                if ($(this).hasClass('bg-gray-400') && $(this).text() !== $('#currentMenu').text()) {
                    $(this).removeClass('bg-gray-400');
                }
            });
        }
    }

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
        // Activate the 'First' tab
        $('#first-tab').trigger('click');

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

        // Set the Trades menu
        if (shouldShowLastTrade === true) {
            $('#spanTrade').text(numberTrades);
        } else if (numberTrades < displayedTradeNumber) {
            $('#spanTrade').text(numberTrades);
        } else {
            $('#spanTrade').text(displayedTradeNumber);
        }

        $('#dropdownBtnTrade').empty();
        let tradesHtml = '';
        for (let i = numberTrades; i > 0; i--) {
            tradesHtml += '<a class="dropdown-item" role="button">' + i + '</a>';
        }
        $('#dropdownBtnTrade').html(tradesHtml);

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

    /**
    * ******************************
    * Region helper functions
    * ******************************
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
            $('#cardBody [data-research-data]').each(function () {
                var bindProperty = $(this).data('research-data');
                var value = $(this).val();

                // Handle CandleType as integer enum value
                if (bindProperty === 'CandleType') {
                    researchData[bindProperty] = value ? parseInt(value) : null;
                }
                // Handle boolean values
                else if (value === "true") {
                    researchData[bindProperty] = true;
                } else if (value === "false") {
                    researchData[bindProperty] = false;
                } else if (value === "") {
                    researchData[bindProperty] = null;
                } else {
                    researchData[bindProperty] = value;
                }
            });
        }

        return researchData;
    }

    /**
    * ******************************
    * Region initialization call
    * ******************************
    */

    initialize();
});

