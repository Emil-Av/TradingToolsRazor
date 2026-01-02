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
    const strategies = {
        Cradle: "Cradle",
        FirstBarPullback: "First Bar Pullback",
        CandleBracketing: "Candle Bracketing",
        SRS: "SRS"
    };

    // Helper function to extract value after colon and trim
    function getSelectValue(selector) {
        var fullText = $(selector + ' option:selected').text();
        if (fullText.includes(':')) {
            return fullText.split(':')[1].trim();
        }
        return fullText.trim();
    }

    // Attach a change event for each select element
    $('#selectTimeFrame, #selectStrategy, #selectTradeType, #selectOrderType').on('change', function () {
        if (this.id === 'selectStrategy') {
            SetResearchPartialView();
        }
    });

    function SetResearchPartialView() {
        if ($('#currentMenu').text() == 'Research') {
            var selectedText = getSelectValue('#selectStrategy');
            if (selectedText == strategies.Cradle) {
                ShowResearchCradlePartialView();
            }
            else if (selectedText == strategies.FirstBarPullback) {
                ShowFirstBarPullbackPartialView();
            }
            else if (selectedText == strategies.CandleBracketing) {
                ShowCandleBracketingPartialView();
            }
            else if (selectedText == strategies.SRS) {
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
        var selectedStrategy = getSelectValue('#selectStrategy');
        // Display TradeData Partial View
        if (currentCardMenu == 'Trade Data') {
            $('#tradeData').removeClass('d-none');
            $('#researchFirstBarPullbackData').addClass('d-none');
            $('#researchCradleData').addClass('d-none');
            $('#researchCandleBracketingData').addClass('d-none');
            $('#researchSRSData').addClass('d-none');

        }
        // Display Research Partial View
        else if (selectedStrategy && selectedStrategy.length > 0) {
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
            var selectedStrategy = getSelectValue('#selectStrategy');
            if (selectedStrategy == strategies.SRS) {
                if (ValidateMenuButtonsAndScreenshots()) {
                    saveTrade();
                }
            }
            else {
                saveTradeOld();
            }
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

    function ValidateMenuButtonsAndScreenshots() {
        var missingSelections = [];
        
        // Check Time Frame
        if ($('#selectTimeFrame').val() === '' || $('#selectTimeFrame').val() === null) {
            missingSelections.push('Time Frame');
        }
        
        // Check Strategy
        if ($('#selectStrategy').val() === '' || $('#selectStrategy').val() === null) {
            missingSelections.push('Strategy');
        }
        
        // Check Trade Type
        if ($('#selectTradeType').val() === '' || $('#selectTradeType').val() === null) {
            missingSelections.push('Type');
        }

        // Validate files
        if (uploadedFiles.length == 0) {
            missingSelections.push("No screenshots uploaded.");
            return;
        }
        
        // If there are missing selections, show error and return false
        if (missingSelections.length > 0) {
            var errorMessage = 'Please select: ' + missingSelections.join(', ');
            toastr.error(errorMessage);
            return false;
        }
        
        return true;
    }
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
        // Collect all data-trade-data values into a viewData object
        var dataViewData = {};
        $('#cardBody [data-trade-data]').each(function () {
            var bindProperty = $(this).data('trade-data');
            var value = $(this).val();
            
            // Handle empty values
            if (value === "") {
                dataViewData[bindProperty] = null;
            } else {
                dataViewData[bindProperty] = value;
            }
        });

        // Collect SampleSizeVM data from menu dropdowns
        var sampleSizeViewData = {
            TimeFrame: parseInt($('#selectTimeFrame').val()),
            Strategy: parseInt($('#selectStrategy').val()),
            TradeType: parseInt($('#selectTradeType').val())
        };

        // Create FormData for multipart/form-data
        var formData = new FormData();
        
        // Add files
        for (var i = 0; i < uploadedFiles.length; i++) {
            formData.append('files', uploadedFiles[i]);
        }
        
        // Add viewData as JSON string
        formData.append('viewData', JSON.stringify(dataViewData));
        
        // Add sampleSizeViewData as JSON string
        formData.append('sampleSizeViewData', JSON.stringify(sampleSizeViewData));

        // Get anti-forgery token
        var token = document.getElementById('__RequestVerificationToken')?.value;

        // Make fetch API call
        fetch('/NewTrade?handler=SaveNewTrade', {
            method: 'POST',
            headers: token ? { 'RequestVerificationToken': token } : {},
            body: formData
        })
        .then(function(response) {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(function(data) {
            if (data.success) {
                toastr.success(data.success);
                clearFields();
            } else if (data.error) {
                toastr.error(data.error);
            }
        })
        .catch(function(error) {
            console.error('Error uploading files:', error);
            toastr.error('Error uploading files. See console for details.');
        });
    }

    function saveTradeOld() {

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
        tradeParams['timeFrame'] = getSelectValue('#selectTimeFrame');
        tradeParams['strategy'] = getSelectValue('#selectStrategy');
        tradeParams['tradeType'] = getSelectValue('#selectTradeType');
        tradeParams['orderType'] = getSelectValue('#selectOrderType');

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
            url: '/NewTrade?handler=SaveNewTradeOld',
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
        $('#cardBody [data-research], #cardBody [data-research-cradle], #cardBody [data-research-firstbar], #cardBody [data-research-bracketing], #cardBody [data-research-srs]').each(function () {
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

        // Reset the menu selects to their placeholder option
        $('#selectTimeFrame, #selectStrategy, #selectTradeType, #selectOrderType').each(function () {
            $(this).prop('selectedIndex', 0);
        });

        // Reset selects that are not part of data-research
        $('select:not([data-research])').not('#selectTimeFrame, #selectStrategy, #selectTradeType, #selectOrderType').each(function () {
            $(this).prop('selectedIndex', 1);
        });
    }


    /**
     * ***************************
     * Region functions ends
     * ***************************
     */

});