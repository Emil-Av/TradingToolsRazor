// Summernote sometimes opens with the cursor in the middle, then that causes errors. Check Review
$(function () {

    /**
    * ******************************
    * Region global variables starts
    * ******************************
    */

    // menuClicked, clickedMenuValue: When a new trade has to be loaded, one of the buttons has to be clicked (either TimeFrame, Strategy..). In case no trade exists for the selection, set the last value. Used in LoadTradeAsync()
    let menuClicked;
    let clickedMenuValue;
    let showLastTrade;
    let loadLastSampleSize = false;
    let statusChanged = false;
    let tradesVM;
    let currentTab = '#pre'; // Always the start value
    let isEditorShown = false;
    let currentCardMenu = 'itemTradingData';

    const tradingData = 'itemTradingData';
    const journal = 'itemJournal';

    /**
    * ******************************
    * Region global variables ends
    * ******************************
    */

    // After a .zip file is uploaded, the 'change' event is triggered, this submits the form and sends the .zip file to the controller
    $('#fileInput').on('change', function () {
        $('#formUploadFile').trigger('submit');
    });


    /**
    * *************************
    * Region summernote starts
    * *************************
    */

    // Send the data to the controller
    function updateReview() {
        let dataToSend =
        {
            CurrentTrade: {
                Id: $('#spanTradeIdInput').val()
            },
            CurrentSampleSize:
            {
                Review: {
                    Id: $('#spanReviewIdInput').val(),
                    First: $('#first').html(),
                    Second: $('#second').html(),
                    Third: $('#third').html(),
                    Forth: $('#forth').html(),
                    summary: $('#summary').html()
                },
                Id: $('#spanSampleSizeIdInput').val(),
            }
        };

        $.ajax({
            method: 'POST',
            url: '/trades/updatereview',
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            data: JSON.stringify(dataToSend),
            success: function (response) {
                if (response['success'] !== undefined) {

                    toastr.success(response['success']);
                }
                else {
                    toastr.error(response['error']);
                }
            }
        });
    }

    function updateTradeData() {

        let tradeData = getTradeData();

        $.ajax({
            method: 'POST',
            url: '/trades/updateTradeData',
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            data: JSON.stringify(tradeData),
            success: function (response) {
                if (response['success'] !== undefined) {

                    toastr.success(response['success']);
                }
                else {
                    toastr.error(response['error']);
                }
            }
        });

    }

    // Send the data to the controller
    function updateJournal() {
        let dataToSend =
        {
            CurrentTrade: {
                Id: $('#spanTradeIdInput').val(),
                JournalId: $('#spanJournalIdInput').val(),
                Journal: {
                    Pre: $('#pre').html(),
                    During: $('#during').html(),
                    Exit: $('#exit').html(),
                    Post: $('#post').html()
                }
            }
        };

        $.ajax({
            method: 'POST',
            url: '/trades/updatejournal',
            contentType: "application/json; charset=utf-8",
            dataType: 'JSON',
            data: JSON.stringify(dataToSend),
            success: function (response) {
                if (response['success'] !== undefined) {
                    toastr.success(response['success']);
                }
                else {
                    toastr.error(response['error']);
                }
            }
        });
    }

    // When the content is double clicked, it can be edited (summernote is displayed)
    $('#tabContentJournal').on('dblclick', function () {
        openEditor();
    });

    $('#tabContentReview').on('dblclick', function () {
        openEditor();
    });
    // On tab change
    $('button[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (isEditorShown) {
            saveEditorText();
        }
        currentTab = '#' + $(e.target).attr('aria-controls');
    });

    $('#btnUpdate').on('click', function () {
        if (validateNumberInputs()) {
            updateTradeData();
        }
    });

    // Open editor and show the buttons
    $('#btnEdit').on('click', function () {
        openEditor();
    });

    // Save the journal changes
    $('#btnSave').on('click', function () {
        saveEditorText();
    });

    // Close the editor and show 'Edit' button
    $('#btnCancel').on('click', function () {
        $('#summernote').summernote('destroy');
        $(currentTab).removeClass('d-none');
        $('#cardBody').addClass('card-body');
        isEditorShown = false;
        toggleFooterButtons();
    });

    function toggleFooterButtons() {
        if ($('#btnEdit').hasClass('d-none')) {
            $('#btnEdit').removeClass('d-none');
            $('.editorOnBtns').addClass('d-none');
        }
        else {
            $('#btnEdit').addClass('d-none');
            $('.editorOnBtns').removeClass('d-none');
        }
    }

    // Save the journal in the DB and toggle the buttons
    function saveEditorText() {
        $('#cardBody').addClass('card-body');
        isEditorShown = false;
        // Save the text from the editor
        let editorText = $($('#summernote').summernote('code')).text().trim();
        // Save the text from the tab
        let oldTabContent = $(currentTab).text().trim();
        // Set the content of the tab to the value of the editor
        $(currentTab).html($('#summernote').summernote('code'));
        // Show the tab content
        $(currentTab).removeClass('d-none');
        // Close the editor
        $('#summernote').summernote('code', '');
        $('#summernote').summernote('destroy');
        toggleFooterButtons();
        // If a change has been made, save it
        if (editorText !== oldTabContent) {
            if (currentCardMenu === 'itemJournal') {
                updateJournal();
            }
            else {
                updateReview();
            }
        }
    }

    // Open the summernote editor
    function openEditor() {
        $('#cardBody').removeClass('card-body');
        toggleFooterButtons();
        // Hide the tabContent of the journal and show the summernote instead
        // Get the text from the tabContent
        let currentTabContent = $(currentTab).html();
        // Hide the tabContent
        $(currentTab).addClass('d-none');
        // Set the text into the editor
        $('#summernote').summernote('code', currentTabContent);
        $('#summernote').summernote('justifyLeft');
        // Display the editor
        isEditorShown = true;
    }

    /**
    * ************************
    * Region summernote ends
    * ************************
    */


    /**
     * ***************************
     * Card dropdown menu starts
     * ***************************
     */

    // Event fired when an item from the dropdown menu in the card header is clicked
    $('#headerMenu').on('click', '.dropdown-item', function () {
        if (isEditorShown) {
            saveEditorText();
        }

        currentCardMenu = $(this).attr('id');

        if (currentCardMenu == tradingData) {
            $('#tradeDataTabHeaders').removeClass('d-none');
            $('#tradeDataTabContent').removeClass('d-none');
            $('#journalTabHeaders').addClass('d-none');
            $('#journalTabContent').addClass('d-none');
            $('#reviewTabHeaders').addClass('d-none');
            $('#reviewTabContent').addClass('d-none');
            $('#btnEdit').addClass('d-none');
            $('#btnUpdate').removeClass('d-none');
        }
        // Display Journal tabs
        else if (currentCardMenu == journal) {
            currentTab = '#pre';
            $('#journalTabHeaders').removeClass('d-none');
            $('#journalTabContent').removeClass('d-none');
            $('#reviewTabHeaders').addClass('d-none');
            $('#reviewTabContent').addClass('d-none');
            $('#tradeDataTabHeaders').addClass('d-none');
            $('#tradeDataTabContent').addClass('d-none');
            $('#btnEdit').removeClass('d-none');
            $('#btnUpdate').addClass('d-none');
        }
        // Display Review tabs
        else {
            currentTab = '#first';
            $('#reviewTabHeaders').removeClass('d-none');
            $('#reviewTabContent').removeClass('d-none');
            $('#journalTabHeaders').addClass('d-none');
            $('#journalTabContent').addClass('d-none');
            $('#tradeDataTabHeaders').addClass('d-none');
            $('#tradeDataTabContent').addClass('d-none');
            $('#btnEdit').removeClass('d-none');
            $('#btnUpdate').addClass('d-none');
        }

        // Set the text of the card menu
        if (currentCardMenu !== $('#currentMenu').text()) {
            let menuText = '';
            if (currentCardMenu == 'itemTradingData') {
                menuText = 'Trade Data';
            }
            else if (currentCardMenu == 'itemJournal') {
                menuText = 'Journal';
            }
            else {
                menuText = 'Review';
            }
            $('#currentMenu').text(menuText);
            $(this).addClass('bg-gray-400');

            // Remove the bg color of the last selected item
            $('#headerMenu a').each(function () {
                // if a dropdown item has the bg-gray-400 class and the text of the current item being iterated is different than the text in the #currentMenu, than that was the previously selected option
                if ($(this).hasClass('bg-gray-400') && $(this).text() !== $('#currentMenu').text()) {
                    $(this).removeClass('bg-gray-400');
                }
            });
        }
    });

    /**
    * ***************************
    * Card dropdown menu ends
    * ***************************
    */


    /**
    * ***************************
    * Region menu buttons starts
    * ***************************
    */
    // Create key, value array: key is the button menu, value is the span element. The span element is the selected value from the dropdown menu.
    let menuButtons =
    {
        '#dropdownBtnStatus': '#spanStatus',
        '#dropdownBtnTimeFrame': '#spanTimeFrame',
        '#dropdownBtnStrategy': '#spanStrategy',
        '#dropdownBtnTradeType': '#spanTradeType',
        '#dropdownBtnSampleSize': '#spanSampleSize',
        '#dropdownBtnTrade': '#spanTrade'
    };

    // Attach a click event for each <a> element of each menu.
    for (let key in menuButtons) {
        (function (key) {
            setSelectedItemClass(key);
            // Change the value of the span in the button
            $(key).on('click', '.dropdown-item', function () {
                // Save the old value. If there is no trade in the DB for the selected trade, the menu's old value should be displayed.
                menuClicked = $(menuButtons[key]);
                clickedMenuValue = $(menuButtons[key]).text();
                // If the time frame,the strategy or the sample size has changed, then the latest trade must always be displayed. Used in SetMenuValues()
                if (key != '#dropdownBtnTrade') {
                    showLastTrade = true;
                }
                else {
                    showLastTrade = false;
                }

                if (key == '#dropdownBtnTimeFrame') {
                    loadLastSampleSize = true;
                }
                else if (key == '#dropdownBtnStatus') {
                    statusChanged = true;
                }
                else {
                    loadLastSampleSize = false;
                    statusChanged = false;
                }

                // Set the new value
                let value = $(this).text();
                $(menuButtons[key]).text(value);
                loadTrade(
                    $('#spanStatus').text(),
                    $('#spanTimeFrame').text(),
                    $('#spanStrategy').text(),
                    $('#spanTradeType').text(),
                    $('#spanSampleSize').text(),
                    $('#spanTrade').text(),
                    showLastTrade,
                    loadLastSampleSize,
                    statusChanged);
            });
        })(key);
    }

    // Mark the selected drop down item of the buttons on the top
    function setSelectedItemClass() {
        for (let key in menuButtons) {
            $(key + ' a').each(function () {
                if ($(this).text() === $(menuButtons[key]).text()) {
                    $(this).addClass('bg-gray-400');
                }
                else {
                    $(this).removeClass('bg-gray-400');
                }
            })
        }
    }
    function loadTrade(status, timeFrame, strategy, tradeType, sampleSize, trade, showLastTrade, loadLastSampleSize, statusChanged) {
        $.ajax({
            method: 'POST',
            url: '/trades/loadtrade',
            dataType: 'JSON',
            data: {
                tradeParams: {
                    StatusFromView: status,
                    TimeFrameFromView: timeFrame,
                    StrategyFromView: strategy,
                    TradeTypeFromView: tradeType,
                    SampleSizeNumberFromView: sampleSize,
                    TradeNumberFromView: trade,
                    ShowLastTradeFromView: showLastTrade,
                    LoadLastSampleSizeFromView: loadLastSampleSize,
                    StatusChangedFromView: statusChanged
                }
            },
            success: function (response) {
                if (response['error'] !== undefined) {
                    toastr.error(response['error']);
                    // Set the old value
                    menuClicked.text(clickedMenuValue);
                    return;
                }
                else if (response['info'] !== undefined) {
                    toastr.info(response['info']);
                    // Set the old value
                    menuClicked.text(clickedMenuValue);
                    return;
                }
                tradesVM = response;
                setHiddenSpansValues(tradesVM);
                loadViewData(trade);

            }
        });
    }

    function setHiddenSpansValues(tradesVM) {
        // Set the new trade id
        $("#spanTradeIdInput").val(tradesVM['tradesVM']['currentTrade']['id']);
        // Set the sample size id
        $("#spanSampleSizeIdInput").val(tradesVM['tradesVM']['currentTrade']['sampleSizeId']);
        $('#spanJournalIdInput').val(tradesVM['tradesVM']['currentTrade']['journalId']);
    }

    function loadViewData(trade) {
        setMenuValues(trade);
        setSelectedItemClass();
        loadImages();
        loadReview();
        loadJournal();
        loadTradeData();
    }

    function loadTradeData() {
        $('#SymbolInput').val(tradesVM['tradesVM']['currentTrade']['symbol']);
        $('#StatusInput').val(tradesVM['tradesVM']['currentTrade']['status']);
        $('#TriggerPriceInput').val(tradesVM['tradesVM']['currentTrade']['triggerPriceInput']);
        $('#EntryPriceInput').val(tradesVM['tradesVM']['currentTrade']['entryPriceInput']);
        $('#StopPriceInput').val(tradesVM['tradesVM']['currentTrade']['stopPriceInput']);
        $('#ExitPriceInput').val(tradesVM['tradesVM']['currentTrade']['exitPriceInput']);
        $('#TargetsInput').val(tradesVM['tradesVM']['currentTrade']['targetsInput']);
        $('#PnLInput').val(tradesVM['tradesVM']['currentTrade']['pnLInput']);
        $('#FeeInput').val(tradesVM['tradesVM']['currentTrade']['feeInput']);
    }

    // Loads the review of the sample size
    function loadReview() {
        // Activate the 'First' tab
        $('#first-tab').trigger('click');
        // Set the values
        $('#first').html(tradesVM['tradesVM']['currentSampleSize']['review']['first']);
        $('#second').html(tradesVM['tradesVM']['currentSampleSize']['review']['second']);
        $('#third').html(tradesVM['tradesVM']['currentSampleSize']['review']['third']);
        $('#forth').html(tradesVM['tradesVM']['currentSampleSize']['review']['forth']);
        $('#summary').html(tradesVM['tradesVM']['currentSampleSize']['review']['summary']);
    }

    // Loads the journal of the trade
    function loadJournal() {
        // Activate the 'Pre' tab
        $('#pre-tab').trigger('click');
        // Set the values
        $('#pre').html(tradesVM['tradesVM']['currentTrade']['journal']['pre']);
        $('#during').html(tradesVM['tradesVM']['currentTrade']['journal']['during']);
        $('#exit').html(tradesVM['tradesVM']['currentTrade']['journal']['exit']);
        $('#post').html(tradesVM['tradesVM']['currentTrade']['journal']['post']);
    }

    // Populate the drop down items after a new trade has been selected and set the values in the spans.
    function setMenuValues(displayedTrade) {
        // Menu Buttons
        let numberSampleSizes = tradesVM['tradesVM']['numberSampleSizes'] !== -1 ? tradesVM['tradesVM']['numberSampleSizes'] : '-';
        let numberTrades = tradesVM['tradesVM']['tradesInSampleSize'] !== 0 ? tradesVM['tradesVM']['tradesInSampleSize'] : tradesVM['tradesVM']['tradesInTimeFrame'];

        // Set the SampleSize menu
        $('#spanSampleSize').text(tradesVM['tradesVM']['currentSampleSizeNumber']);
        $('#dropdownBtnSampleSize').empty();
        let sampleSizes = '';
        for (let i = numberSampleSizes; i > 0; i--) {
            sampleSizes += '<a class="dropdown-item" role="button">' + i + '</a>';
        }
        $('#dropdownBtnSampleSize').html(sampleSizes);


        // Set the Trades menu
        if (showLastTrade === true) {
            $('#spanTrade').text(numberTrades);
        }
        else if (numberTrades < displayedTrade) {
            $('#spanTrade').text(numberTrades);
        }
        else {
            $('#spanTrade').text(displayedTrade);
        }

        $('#dropdownBtnTrade').empty();
        let trades = '';
        for (let i = numberTrades; i > 0; i--) {
            trades += '<a class="dropdown-item" role="button">' + i + '</a>'
        }
        $('#dropdownBtnTrade').html(trades);

        setTimeFrameMenu(tradesVM['tradesVM']['availableTimeframes'], GetTimeFrameMapping(), tradesVM['tradesVM']['currentSampleSize']['timeFrame']);

        // Menu card header
        currentCardMenu = journal;
        $('#cardMenuTradeData').trigger('click');
    }

    function GetTimeFrameMapping() {
        const timeFrames = {
            "M5": "5M",
            "M10": "10M",
            "M15": "15M",
            "M30": "30M",
            "H1": "1H",
            "H2": "2H",
            "H4": "4H",
            "D": "D"
        }

        return timeFrames;
    }

    // Load the images into the carousel
    function loadImages() {
        $('#imageContainer').empty();
        let screenshots = tradesVM['tradesVM']['currentTrade']['screenshotsUrls'];

        let newCarouselHtml = '<ol class="carousel-indicators">';
        for (let i = 0; i < screenshots.length; i++) {
            let url = screenshots[i];
            if (i == 0) {
                newCarouselHtml += '<li data-bs-target="#carouselTrades" data-slide-to="' + i + '" class="active"></li >';
            }
            else {
                newCarouselHtml += '<li data-bs-target="#carouselTrades" data-slide-to="' + i + '" ></li >';
            }
        }
        newCarouselHtml += '</ol>';

        newCarouselHtml += '<div class="carousel-inner">';
        for (let i = 0; i < screenshots.length; i++) {
            let url = screenshots[i];
            if (i == 0) {
                newCarouselHtml += '<div class="carousel-item active"><img src="' + url + '" class="d-block w-100" alt = "..." ></div>';
            }
            else {
                newCarouselHtml += '<div class="carousel-item"><img src="' + url + '" class="d-block w-100" alt = "..." ></div>';;
            }
        }
        newCarouselHtml += '</div>';

        $('#imageContainer').html(newCarouselHtml);
    }

    /**
    * ***************************
    * Region menu buttons ends
    * ***************************
    */
})

