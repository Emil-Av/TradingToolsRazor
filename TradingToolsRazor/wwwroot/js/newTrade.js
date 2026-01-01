$(function () {
    /**
   * ******************************
   * Region global variables starts
   * ******************************
   */

    var uploadedFiles = [];

    /**
      * ****************************
      * Region global variables ends
      * ****************************
      */


    /**
    * ***************************
    * Region menu buttons starts
    * ***************************
    */
    // Create key, value array: key is the button menu, value is the span element. The span element is the selected value from the dropdown menu.
    var menuButtons =
    {
        '#dropdownBtnStatus': '#spanStatus',
        '#dropdownBtnTimeFrame': '#spanTimeFrame',
        '#dropdownBtnStrategy': '#spanStrategy',
        '#dropdownBtnTradeType': '#spanTradeType',
        '#dropdownBtnOrderType': '#spanOrderType'
    };

    const strategies = {
        Cradle: "Cradle",
        FirstBarPullback: "First Bar Pullback",
        CandleBracketing: "Candle Bracketing",
        SRS: "SRS"
    };

    // Attach a click event for each <a> element of each menu.
    for (var key in menuButtons) {
        (function (key) {
            // Change the value of the span in the button
            $(key).on('click', '.dropdown-item', function () {
                // Set the new value
                var value = $(this).text();
                $(menuButtons[key]).text(value);
                SetSelectedItemClass(key);
                if (key == '#dropdownBtnStrategy') {
                    SetResearchPartialView();
                    if ($('#spanStrategy').text() == strategies.SRS) {
                        SetSymbolDropDown();
                    }
                }

            });
        })(key);
    }

    function SetSymbolDropDown() {
        const symbols = {
            0: 'DAX',
            1: 'DOW'
        };

        const $symbolInput = $('#InputSymbol');
        
        // Check if it's already a select element
        if ($symbolInput.is('select')) {
            return;
        }
        
        // Get current value and attributes
        const currentValue = $symbolInput.val();
        const dataAttribute = $symbolInput.attr('data-trade-data');
        const classes = $symbolInput.attr('class');
        
        // Create select element
        let $select = $('<select>', {
            id: 'InputSymbol',
            'data-trade-data': dataAttribute,
            class: classes
        });
        
        // Add enum options
        for (const [value, name] of Object.entries(symbols)) {
            const selected = currentValue == name ? 'selected' : '';
            $select.append(`<option value="${value}" ${selected}>${name}</option>`);
        }
        
        // Replace input with select
        $symbolInput.replaceWith($select);
    }

    function SetResearchPartialView() {
        if ($('#currentMenu').text() == 'Research') {
            if ($('#spanStrategy').text() == strategies.Cradle) {
                ShowResearchCradlePartialView();
            }
            else if ($('#spanStrategy').text() == strategies.FirstBarPullback) {
                ShowFirstBarPullbackPartialView();
            }
            else if ($('#spanStrategy').text() == strategies.CandleBracketing) {
                ShowCandleBracketingPartialView();
            }
            else if ($('#spanStrategy').text() == strategies.SRS) {
                ShowSRSPartialView();
            }
        }
    }

    function ShowSRSPartialView() {
        $('#researchSRSData').removeClass('d-none');

        $('#researchCandleBracketingData').addClass('d-none');
        $('#researchCradleData').addClass('d-none');
        $('#researchFirstBarPullbackData').addClass('d-none');
    }

    function ShowCandleBracketingPartialView() {
        $('#researchCandleBracketingData').removeClass('d-none');
        $('#researchCradleData').addClass('d-none');
        $('#researchFirstBarPullbackData').addClass('d-none');
        $('#researchSRSData').addClass('d-none');
    }

    function ShowResearchCradlePartialView() {
        $('#researchCradleData').removeClass('d-none');

        $('#researchFirstBarPullbackData').addClass('d-none');
        $('#researchCandleBracketingData').addClass('d-none');
        $('#researchSRSData').addClass('d-none');
    }

    function ShowFirstBarPullbackPartialView() {
        $('#researchFirstBarPullbackData').removeClass('d-none');

        $('#researchCradleData').addClass('d-none');
        $('#researchCandleBracketingData').addClass('d-none');
        $('#researchSRSData').addClass('d-none');
    }

    // Mark the selected drop down item of the buttons on the top
    function SetSelectedItemClass() {
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
     * Region event handlers begins
     * ***************************
     */

    $('#headerMenu').on('click', '.dropdown-item', function () {
        var currentCardMenu = $(this).text();
        var selectedStrategy = $('#spanStrategy').text();
        // Display TradeData Partial View
        if (currentCardMenu == 'Trade Data') {
            $('#tradeData').removeClass('d-none');
            $('#researchFirstBarPullbackData').addClass('d-none');
            $('#researchCradleData').addClass('d-none');
            $('#researchCandleBracketingData').addClass('d-none');
            $('#researchSRSData').addClass('d-none');

        }
        // Display Research Partial View
        else if (selectedStrategy.length > 0) {
            if (selectedStrategy == "Cradle") {
                ShowResearchCradlePartialView();
                $('#tradeData').addClass('d-none');
            }
            else if (selectedStrategy == strategies.FirstBarPullback) {
                ShowFirstBarPullbackPartialView();
                $('#tradeData').addClass('d-none');
            }
            else if (selectedStrategy == strategies.CandleBracketing) {
                ShowCandleBracketingPartialView();
                $('#tradeData').addClass('d-none');
            }
            else if (selectedStrategy == strategies.SRS) {
                ShowSRSPartialView();
                $('#tradeData').addClass('d-none');
            }
        }
        else {
            currentCardMenu = 'Trade Data';
            toastr.info('Choose strategy first.');
        }

        // Set the text of the card menu
        if (currentCardMenu !== $('#currentMenu').text()) {
            $('#currentMenu').text(currentCardMenu);
            $(this).addClass('bg-gray-400');
        }

        // Remove the bg color of the last selected item
        $('#headerMenu a').each(function () {
            // if a dropdown item has the bg-gray-400 class and the text of the current item being iterated is different than the text in the #currentMenu, than that was the previously selected option
            if ($(this).hasClass('bg-gray-400') && $(this).text() !== $('#currentMenu').text()) {
                $(this).removeClass('bg-gray-400');
            }
        });
    });

    $('#btnSave').on('click', function () {
        if (validateNumberInputs()) {
            saveTrade();
        }
    });

    $('#btnClear').on('click', function () {
        clearFields();
    });

    // After uploading screenshots, the 'change' event is triggered. Save the files in the global variable and display the file names
    $('input[type="file"]').change(function (e) {
        var files = $(this)[0].files; // Get the files array from the input element

        // Iterate through each selected file to append it to FormData
        for (var i = 0; i < files.length; i++) {
            uploadedFiles.push(files[i]);
        }
        // Sort the screenshots to be in ascending order based on the lastModified property
        uploadedFiles.sort(function (a, b) {
            return a.lastModified - b.lastModified;
        });

        displayNames();
    });

    /**
     * ***************************
     * Region event handlers ends
     * ***************************
     */

    /**
     * ***************************
     * Region functions begins
     * ***************************
     */
    function displayNames() {
        var fileList = $('#fileList');
        fileList.removeClass('text-center').addClass('text-left');
        fileList.empty();

        for (var i = 0; i < uploadedFiles.length; i++) {
            fileName = (i + 1) + '. ' + uploadedFiles[i].name;
            fileList.append('<p class="text-truncate">' + fileName + '</p>');
        }
    }

    function saveTrade() {

        var formData = new FormData();

        // Trades without screenshots should not be saved
        if (uploadedFiles.length == 0) {
            toastr.error("No screenshots uploaded.");
            return;
        }

        for (var i = 0; i < uploadedFiles.length; i++) {
            formData.append('files', uploadedFiles[i]);
        }

        var tradeParams = {};
        tradeParams['status'] = $('#spanStatus').text();
        tradeParams['timeFrame'] = $('#spanTimeFrame').text();
        tradeParams['strategy'] = $('#spanStrategy').text();
        tradeParams['tradeType'] = $('#spanTradeType').text();
        tradeParams['orderType'] = $('#spanOrderType').text();

        formData.append('tradeParams', JSON.stringify(tradeParams));

        var researchData = {};
        if (tradeParams['strategy'] == strategies.Cradle) {
            $('#cardBody [data-research-cradle]').each(function () {
                var bindProperty = $(this).data('research-cradle');
                researchData[bindProperty] = $(this).val();
            });
        }
        else if (tradeParams['strategy'] == strategies.FirstBarPullback) {

            $('#cardBody [data-research-firstbar]').each(function () {
                var bindProperty = $(this).data('research-firstbar');
                researchData[bindProperty] = $(this).val();
            });
        }
        else if (tradeParams['strategy'] == strategies.CandleBracketing) {
            $('#cardBody [data-research-bracketing]').each(function () {
                var bindProperty = $(this).data('research-bracketing');
                researchData[bindProperty] = $(this).val();
            });
        }

        else if (tradeParams['strategy'] == strategies.SRS) {
            $('#cardBody [data-research-srs]').each(function () {
                var bindProperty = $(this).data('research-srs');
                researchData[bindProperty] = $(this).val();
            });
        }

        formData.append('researchData', JSON.stringify(researchData));

        var tradeData = getTradeData();

        formData.append('tradeData', JSON.stringify(tradeData));

        var token = document.getElementById('__RequestVerificationToken')?.value;
        $.ajax({
            type: 'POST',
            url: '/NewTrade?handler=SaveNewTrade',
            processData: false,
            contentType: false,
            headers: token ? { 'RequestVerificationToken': token } : {},
            data: formData,
            success: function (response) {
                if (response['success'] !== undefined) {
                    toastr.success(response['success']);
                    clearFields();
                }
                else if (response['error'] !== undefined) {
                    toastr.error(response['error']);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error uploading files:', jqXHR, textStatus, errorThrown);
                toastr.error('Error uploading files. See console for details.');
            }
        });
    };

    function clearFields() {
        // Helpers: format date/time for HTML inputs
        function formatDateForInput(date) {
            // yyyy-MM-dd
            return date.toISOString().split('T')[0];
        }
        function formatTimeForInput(date) {
            // HH:mm
            var hours = String(date.getHours()).padStart(2, '0');
            var minutes = String(date.getMinutes()).padStart(2, '0');
            return hours + ':' + minutes;
        }

        var now = new Date();
        var todayValue = formatDateForInput(now);
        var timeValue = formatTimeForInput(now);

        // Clear and set sensible defaults for elements that belong to research partials
        $('#cardBody [data-research], #cardBody [data-research-cradle], #cardBody [data-research-firstbar], #cardBody [data-research-bracketing]').each(function () {
            var element = $(this);
            if (element.is('input')) {
                var elementType = (element.attr('type') || '').toLowerCase();
                if (elementType === 'date') {
                    element.val(todayValue);
                } else if (elementType === 'time') {
                    element.val(timeValue);
                } else if (elementType === 'number' || element.hasClass('number-input')) {
                    element.val('0');
                } else {
                    element.val('');
                }
            } else if (element.is('select')) {
                element.prop('selectedIndex', 1);
            } else if (element.is('textarea')) {
                element.val('');
            }
        });

        // Clear visible inputs outside research area but DO NOT clear hidden inputs (antiforgery token etc.)
        $('input:not([data-research]):not([type="hidden"])').each(function () {
            var element = $(this);
            var elementType = (element.attr('type') || '').toLowerCase();
            if (elementType === 'date') {
                element.val(todayValue);
            } else if (elementType === 'time') {
                element.val(timeValue);
            } else if (elementType === 'number' || element.hasClass('number-input')) {
                element.val('0');
            } else {
                element.val('');
            }
        });

        // Reset file list UI and file inputs
        $('#fileList').empty();
        $('#fileList').append('<p class="text-truncate">No uploaded files.</p>');
        $('#formUploadFiles').trigger('reset');
        uploadedFiles = [];

        // Reset selects that are not part of data-research
        $('select:not([data-research])').each(function () {
            $(this).prop('selectedIndex', 1);
        });
    }


    /**
     * ***************************
     * Region functions ends
     * ***************************
     */

});