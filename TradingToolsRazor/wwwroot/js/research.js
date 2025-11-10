
$(function () {
    // After a .zip file is uploaded, the 'change' event is triggered, this submits the form and sends the .zip file to the controller
    $('#fileInput').on('change', function () {
        $('#formUploadFile').trigger('submit');
    });

    /**
    * ******************************
    * Region global variables starts
    * ******************************
    */

    // menuClicked, clickedMenuValue: When a new trade has to be loaded, one of the buttons has to be clicked (either TimeFrame, Strategy..). In case no trade exists for the selection, set the last value. Used in LoadTradeAsync()
    // Global index for the currently displayed trade. Can be used in the 'trades' variable
    var tradeIndex = 0;
    var lastTradeIndex = 0;
    var currentCardMenu = 'Trade Data';
    // The model
    var researchVM;
    var trades = $('#tradesData').data('trades');

    const strategies = {
        FirstBarPullback: 'First Bar Pullback',
        Cradle: 'Cradle',
        CandleBracketing: 'Candle Bracketing'
    };

    /**
    * ******************************
    * Region global variables ends
    * ******************************
    */


    function setMenuValues(researchVM) {
        // Menu Buttons
        var tradesInSampleSize = researchVM.TradesInSampleSize;
        $('#tradesInSampleSize').val('/' + tradesInSampleSize);
        // Set the SampleSize menu
        $('#spanSampleSize').text(researchVM.CurrentSampleSizeNumber);
        setDropdownBtnSampleSize(researchVM);
        setTimeFrameMenu(researchVM['AvailableTimeframes'], GetTimeFrameMapping(), researchVM['CurrentSampleSize']['TimeFrame']);
    }

    function GetTimeFrameMapping() {
        const timeFrames = {
            0: "5M",   // ETimeFrame.M5
            1: "10M",  // ETimeFrame.M10
            2: "15M",  // ETimeFrame.M15
            3: "30M",  // ETimeFrame.M30
            4: "1H",   // ETimeFrame.H1
            5: "2H",   // ETimeFrame.H2
            6: "4H",   // ETimeFrame.H4
            7: "D"     // ETimeFrame.D
        };

        return timeFrames;
    }

    function setDropdownBtnSampleSize(researchVM) {
        $('#dropdownBtnSampleSize').empty();
        var sampleSizes = '';
        for (var i = researchVM.NumberSampleSizes; i > 0; i--) {
            sampleSizes += '<a class="dropdown-item" role="button">' + i + '</a>';
        }

        $('#dropdownBtnSampleSize').html(sampleSizes);
    }

    /**
     * ***************************
     * Region menu buttons starts
     * ***************************
     */
    // Create key, value array: key is the button menu, value is the span element. The span element is the selected value from the dropdown menu.
    var menuButtons =
    {
        '#dropdownBtnTimeFrame': '#spanTimeFrame',
        '#dropdownBtnStrategy': '#spanStrategy',
        '#dropdownBtnSampleSize': '#spanSampleSize'
    };

    // Attach a click event for each <a> element of each menu.
    for (var key in menuButtons) {
        (function (key) {
            setSelectedItemClass(key);
            // Change the value of the span in the button
            $(key).on('click', '.dropdown-item', function () {
                // Save the old value. If there is no trade in the DB for the selected trade, the menu's old value should be displayed.
                menuClicked = $(menuButtons[key]);
                clickedMenuValue = $(menuButtons[key]).text();
                var sampleSizeChanged = false;
                var timeFrameChanged = false;
                var strategyChanged = false;

                if (key == '#dropdownBtnSampleSize') {
                    sampleSizeChanged = true;
                }
                if (key == '#dropdownBtnTimeFrame') {
                    timeFrameChanged = true;
                }
                if (key == '#dropdownBtnStrategy') {
                    strategyChanged = true;
                }

                // Set the new value
                var value = $(this).text();
                $(menuButtons[key]).text(value);
                loadSampleSizeAsync($('#spanTimeFrame').text(),
                    $('#spanStrategy').text(),
                    $('#spanSampleSize').text(),
                    sampleSizeChanged,
                    timeFrameChanged,
                    strategyChanged);
            });
        })(key);
    }

    // Mark the selected drop down item of the buttons on the top
    function setSelectedItemClass() {
        // Set the "selected item" color
        for (var key in menuButtons) {
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

    /**
    * ***************************
    * Region menu buttons ends
    * ***************************
    */


    /**
     * ***************************
     * Region even handlers begins
     * ***************************
     */

    $('#headerMenu').on('click', '.dropdown-item', function () {
        currentCardMenu = $(this).text();
        // Display Journal tabs
        if (currentCardMenu == 'Trade Data') {
            currentTab = '#pre';
            $('#tradeDataTabHeaders').removeClass('d-none');
            $('#tradeDataTabContent').removeClass('d-none');
            $('#researchDataTabHeaders').addClass('d-none');
            $('#researchDataTabContent').addClass('d-none');
        }
        // Display Review tabs
        else {
            currentTab = '#first';
            $('#researchDataTabHeaders').removeClass('d-none');
            $('#researchDataTabContent').removeClass('d-none');
            $('#tradeDataTabHeaders').addClass('d-none');
            $('#tradeDataTabContent').addClass('d-none');
        }

        // Set the text of the card menu
        if (currentCardMenu !== $('#currentMenu').text()) {
            $('#currentMenu').text(currentCardMenu);
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


    // Card button 'Update' click event handler 
    $('#btnUpdate').on('click', function () {
        updateTradeData(tradeIndex);
    });

    // Card button 'Delete' click event handler 
    $('#btnDelete').on('click', function () {
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
                deleteTrade();
            }
        });

    });

    // Menu button "Next" click event handler
    $('#btnNext').on('click', function () {
        showNextTrade(1);
    });

    // Menu button "Previous" click event handler
    $('#btnPrev').on('click', function () {
        showPrevTrade(-1);
    });

    // Event handler when the left or the right arrow is pressed. Displays the trade accordingly.
    $(document).on('keydown', function (event) {
        // Left arrow key pressed
        if (event.which === 37) {
            showPrevTrade(-1);
            // Right arrow key pressed
        } else if (event.which === 39) {
            showNextTrade(1);
        }
    });

    // User enters trade number and presses enter
    $('#tradeNumberInput').on('keypress', function (event) {
        if (event.which === 13) {
            userInput = Number(event.target.value);
            if (Number.isInteger(userInput)) {
                tradeIndex = userInput - 1;
                displayTradeData(tradeIndex, true);
            }
            else {
                toastr.error("Please enter a whole number.");
                return
            }
        }
    });


    /**
     * ***************************
     * Region event handlers ends
     * ***************************
     */

    /**
     * ***************************
     * Region methods begins
     * ***************************
     */

    // Toggles to the next trade
    function showNextTrade(index) {
        displayTradeData(index, false);
    }
    // Toggles to the previous trade
    function showPrevTrade(index) {
        displayTradeData(index, false);
    }

    // Loads the screenshots and the values in the input/select elements in the card
    function displayTradeData(indexToShow, isUserInput) {
        // Buttons 'prev' or 'next'
        if (!isUserInput && (indexToShow == -1 || indexToShow == 1)) {
            tradeIndex += indexToShow;
        }
        // User input of the trade to be shown
        else {
            tradeIndex = indexToShow;
        }

        if (isUserInput && tradeIndex >= trades.length) {
            displayToastrErrorWhenSwitchingTrades('Trade ' + (tradeIndex + 1) + ' doesn\'t exist.');
            return;
        }
        else if (isUserInput && tradeIndex < 0) {
            displayToastrErrorWhenSwitchingTrades('Trade number can\'t be smaller then 1.');
            return;
        }

        else if (!isUserInput && tradeIndex >= trades.length) {
            displayToastrInfoWhenSwitchingTrades('The last trade is being displayed');
            return;
        }
        else if (!isUserInput && tradeIndex < 0) {
            displayToastrInfoWhenSwitchingTrades('The first trade is being displayed');
            return;
        }
        lastTradeIndex = tradeIndex;
        $('#tradeNumberInput').val(tradeIndex + 1);
        loadImages();
        loadTradeData();
    }

    function displayToastrErrorWhenSwitchingTrades(message) {
        toastr.error(message);
    }

    function displayToastrInfoWhenSwitchingTrades(message) {
        toastr.info(message);
        tradeIndex = lastTradeIndex;
    }

    function deleteTrade() {
        var currentTradeNumber = parseInt($('#tradeNumberInput').val());
        var id = getTradeId(currentTradeNumber);
        setCurrentIndex(currentTradeNumber)
        ajaxDelete(id)
    }

    function getStrategyAsEnumIntValue() {
        if ($('#spanStrategy').text() == strategies.FirstBarPullback) {
            return 0;
        }
        return 1;
    }

    async function ajaxDelete(id) {
        const token = document.getElementById('__RequestVerificationToken')?.value;
        const strategy = getStrategyAsEnumIntValue();
        const url = `/Research?handler=Delete&id=${encodeURIComponent(id)}&strategy=${encodeURIComponent(strategy)}`;

        try {
            const resp = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json',
                    ...(token ? { 'RequestVerificationToken': token } : {})
                }
            });

            if (!resp.ok) {
                const text = await resp.text();
                createAjaxErrorMsg({ status: resp.status, responseText: text }, resp.statusText);
                return;
            }

            const response = await resp.json();

            if (response['error'] !== undefined) {
                toastr.error(response['error']);
                return;
            }

            if (response.redirectUrl) {
                window.location.href = response.redirectUrl;
                return;
            }

            setData(response);
        } catch (ex) {
            createAjaxErrorMsg({ status: 0, responseText: ex.message }, ex.message);
        }
    }

    function setCurrentIndex(currentTradeNumber) {
        if (currentTradeNumber - 1 <= 0) {
            tradeIndex = 0;
        }
        else {
            tradeIndex = currentTradeNumber - 1;
        }
    }

    function createAjaxErrorMsg(jqXHR, exception) {
        var msg = '';
        if (jqXHR.status === 0) {
            msg = 'Not connect.\n Verify Network.';
        } else if (jqXHR.status == 404) {
            msg = 'Requested page not found. [404]';
        } else if (jqXHR.status == 500) {
            msg = 'Internal Server Error [500].';
        } else if (exception === 'parsererror') {
            msg = 'Requested JSON parse failed.';
        } else if (exception === 'timeout') {
            msg = 'Time out error.';
        } else if (exception === 'abort') {
            msg = 'Ajax request aborted.';
        } else {
            msg = 'Uncaught Error.\n' + jqXHR.responseText;
        }
        alert(msg);
    }

    // Updates the database with the values from the card for the displayed trade
    function updateTradeData(index) {
        var updatedResearch = {};
        var strategy = $('#spanStrategy').text();
        if (strategy == strategies.FirstBarPullback) {
            updatedResearch = getFirstBarResearchData(index);
            updateFirstBarResearch(updatedResearch);
        }
        else if (strategy == strategies.Cradle) {
            updatedResearch = getCradleResearchData(index);
            updateCradleResearch(updatedResearch);
        }
    }

    async function updateCradleResearch(updatedResearch) {
        const token = document.getElementById('__RequestVerificationToken')?.value;
        const url = '/Research?handler=UpdateCradleResearch';

        try {
            const resp = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json; charset=utf-8',
                    ...(token ? { 'RequestVerificationToken': token } : {})
                },
                body: JSON.stringify(updatedResearch)
            });

            if (!resp.ok) {
                const text = await resp.text();
                createAjaxErrorMsg({ status: resp.status, responseText: text }, resp.statusText);
                return;
            }

            const json = await resp.json();
            if (json.success) {
                toastr.success(json.success);
            } else if (json.error) {
                toastr.error(json.error);
            }
        } catch (ex) {
            console.error(ex);
            toastr.error('Request failed. See console for details.');
        }
    }

    async function updateFirstBarResearch(updatedResearch) {
        const token = document.getElementById('__RequestVerificationToken')?.value;
        const url = '/Research?handler=UpdateFirstBarResearch';

        try {
            const resp = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json; charset=utf-8',
                    ...(token ? { 'RequestVerificationToken': token } : {})
                },
                body: JSON.stringify(updatedResearch)
            });

            if (!resp.ok) {
                const text = await resp.text();
                createAjaxErrorMsg({ status: resp.status, responseText: text }, resp.statusText);
                return;
            }

            const json = await resp.json();
            if (json.success) {
                toastr.success(json.success);
            } else if (json.error) {
                toastr.error(json.error);
            }
        } catch (ex) {
            console.error(ex);
            toastr.error('Request failed. See console for details.');
        }
    }

    function getCradleResearchData(index) {
        var updatedTrade = {};
        $('#cardBody [data-research-cradle]').each(function () {
            var bindProperty = $(this).data('research-cradle');
            updatedTrade[bindProperty] = $(this).val();
        });

        //updatedTrade['TradeRating'] = parseInt($('#TradeRatingInput').val());
        updatedTrade['Id'] = trades[index]['Id'];
        updatedTrade['ScreenshotsUrls'] = trades[index]['ScreenshotsUrls'];

        return updatedTrade;
    }

    function getFirstBarResearchData(index) {
        var updatedTrade = {};
        $('#cardBody [data-research-firstbar]').each(function () {
            var bindProperty = $(this).data('research-firstbar');
            updatedTrade[bindProperty] = $(this).val();
        });

        // Add the Id and the Screenshots
        updatedTrade['TradeRatingDisplay'] = parseInt($('#TradeRatingInput').val());
        updatedTrade['IdDisplay'] = trades[index]['IdDisplay'];
        updatedTrade['ScreenshotsUrls'] = trades[index]['ScreenshotsUrls'];

        return updatedTrade;
    }

    // Loads the trade data into the input/select elements. Used in the prev/next buttons or the key combination
    function loadTradeData(strategy) {
        if (strategy === undefined) {
            strategy = getStrategy();
        }
        var trade = trades[tradeIndex];
        $('#currentTradeId').val(trade['Id']);
        if (strategy == 0) {
            setFirstBarResearchData(trade);
        }
        else if (strategy == 1) {
            setCradleResearchData(trade);
        }
        else if (strategy == 2) {
            setCandleBracketingData(trade);
        }
    }

    function getStrategy() {
        var strategy = $('#spanStrategy').text();
        if (strategy == strategies.FirstBarPullback) {
            return 0;
        }
        else if (strategy == strategies.Cradle) {
            return 1;
        }
        else if (strategy == strategies.CandleBracketing) {
            return 2;
        }
    }

    function setCandleBracketingData(trade) {
        $('#cardBody [data-research-bracketing]').each(function () {
            var bindProperty = $(this).data('research-bracketing');
            if (trade.hasOwnProperty(bindProperty)) {
                var value = trade[bindProperty];
                if (value !== null) {
                    $(this).val(trade[bindProperty].toString());
                }
            }
        });
    }

    function setCradleResearchData(trade) {
        $('#cardBody [data-research-cradle]').each(function () {
            var bindProperty = $(this).data('research-cradle');
            if (trade.hasOwnProperty(bindProperty)) {
                $(this).val(trade[bindProperty]);
            }
        });
    }

    function setFirstBarResearchData(trade) {
        $('#cardBody [data-research-firstbar]').each(function () {
            var bindProperty = $(this).data('research-firstbar');
            if (trade.hasOwnProperty(bindProperty)) {
                $(this).val(trade[bindProperty]);
            }
        });
    }
    // Loads the images into the carousel
    function loadImages() {
        var screenshots = trades[tradeIndex]['ScreenshotsUrls'];
        if (screenshots === null) {
            toastr.error("No screenshots for the selected trade.");
            return;
        }

        var newCarouselHtml = '<ol class="carousel-indicators">';
        for (var i = 0; i < screenshots.length; i++) {
            var url = screenshots[i];
            if (i == 0) {
                newCarouselHtml += '<li data-bs-target="#carouselTrades" data-bs-slide-to="' + i + '" class="active"></li >';
            }
            else {
                newCarouselHtml += '<li data-bs-target="#carouselTrades" data-bs-slide-to="' + i + '" ></li >';
            }
        }
        newCarouselHtml += '</ol>';

        newCarouselHtml += '<div class="carousel-inner">';
        for (var i = 0; i < screenshots.length; i++) {
            var url = screenshots[i];
            if (i == 0) {
                newCarouselHtml += '<div class="carousel-item active"><img src="' + url + '" class="d-block w-100" alt = "..." ></div>';
            }
            else {
                newCarouselHtml += '<div class="carousel-item"><img src="' + url + '" class="d-block w-100" alt = "..." ></div>';;
            }
        }
        newCarouselHtml += '</div>';
        $('#imageContainer').empty();
        $('#imageContainer').html(newCarouselHtml);
        $('#tradeNumberInput').val(tradeIndex + 1);

        console.log(newCarouselHtml);
    }

    async function loadSampleSizeAsync(timeFrame, strategy, sampleSizeNumber, isSampleSizeChanged, isTimeFrameChanged, isStrategyChanged) {
        const token = document.getElementById('__RequestVerificationToken')?.value;
        const url = '/Research?handler=LoadSampleSize';

        const params = new URLSearchParams();
        params.append('TimeFrame', timeFrame);
        params.append('Strategy', strategy);
        params.append('SampleSizeNumber', sampleSizeNumber);
        params.append('IsSampleSizeChanged', isSampleSizeChanged);
        params.append('IsTimeFrameChanged', isTimeFrameChanged);
        params.append('IsStrategyChanged', isStrategyChanged);
        if (token) params.append('__RequestVerificationToken', token);

        try {
            const resp = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                    ...(token ? { 'RequestVerificationToken': token } : {})
                },
                body: params.toString()
            });

            if (!resp.ok) {
                const text = await resp.text();
                createAjaxErrorMsg({ status: resp.status, responseText: text }, resp.statusText);
                return;
            }

            const response = await resp.json();
            if (response?.error !== undefined) {
                toastr.error(response.error);
                return;
            }

            setData(response);
        } catch (ex) {
            console.error(ex);
            toastr.error('Request failed. See console for details.');
        }
    }

    function setData(response) {
        researchVM = JSON.parse(response.researchVM);
        tradeIndex = 0;
        trades = researchVM.AllTrades;
        showResearchData(researchVM['CurrentStrategy']);
        loadTradeData(researchVM['CurrentStrategy']);
        loadImages();
        setMenuValues(researchVM);
        setSelectedItemClass();
    }

    function showResearchData(strategy) {
        if (strategy == 0) {
            $('#researchDataContainer').html(partialViewFirstBarResearch);

        }
        else if (strategy == 1) {
            $('#researchDataContainer').html(partialViewCradleResesarch);
        }
        else if (strategy == 2) {
            $('#researchDataContainer').html(partialViewCandleBracketingResearch);
        }
    }

    function getTradeId(currentTradeNumber) {
        if ($('#spanStrategy').text() == strategies.FirstBarPullback) {
            return trades[currentTradeNumber - 1]['IdDisplay'];
        }
        return trades[currentTradeNumber - 1]['Id'];
    }

    /**
    * ***************************
    * Region methods
    * ***************************
    */
});