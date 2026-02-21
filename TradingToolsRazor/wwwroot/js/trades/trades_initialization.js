/**
 * ******************************
 * Region initialization
 * ******************************
 */

function initialize() {
    initializeEventHandlers();
    initializeMenuButtons();
    initializeTradeNavigation();
}

function initializeEventHandlers() {
    // Summernote editor events
    $('#tabContentJournal').on('dblclick', openEditor);
    $('#tabContentReview').on('dblclick', openEditor);
    $('button[data-bs-toggle="tab"]').on('shown.bs.tab', handleTabChange);

    // Button click events
    $('#ButtonUpdate').on('click', handleUpdateClick);
    $('#ButtonDelete').on('click', handleDeleteClick);
    $('#ButtonEdit').on('click', openEditor);
    $('#ButtonSave').on('click', saveEditorText);
    $('#ButtonCancel').on('click', handleCancelClick);

    // File input change event for screenshot upload
    $('#InputUploadScreenshot').on('change', handleScreenshotFileChange);

    // Card menu events
    $('#headerMenu').on('click', '.dropdown-item', handleCardMenuClick);

    // Trade navigation events
    $('#btnNext').on('click', handleNextClick);
    $('#btnPrev').on('click', handlePrevClick);
    $('#tradeNumberInput').on('keypress', handleTradeNumberInput);
    $(document).on('keydown', handleKeyboardNavigation);
}

function initializeMenuButtons() {
    // Attach change event handlers for each select element
    $('#selectStrategy, #selectSampleSizeType, #selectStatus, #selectTimeFrame, #selectSampleSize').on('change', function() {
        const selectId = this.id;
        const selectedValue = $(this).val();
        const selectedText = $(this).find('option:selected').text();
        
        // Handle specific select changes
        handleMenuSelectChange(selectId, selectedValue, selectedText);
    });
}

function initializeTradeNavigation() {
    // Load all trades from the serialized data
    if (typeof window.allTradesData !== 'undefined' && window.allTradesData) {
        allTrades = window.allTradesData;
        console.log('Loaded ' + allTrades.length + ' trades for client-side navigation');
    }
    
    // Get the current trade number from the input field
    const currentTradeNumber = parseInt($('#tradeNumberInput').val()) || 1;
    const totalTrades = allTrades.length || parseInt($('#tradesInSampleSize').val()?.replace('/', '')) || 0;
    
    initializeTradeIndex(currentTradeNumber);
    updateTotalTradesDisplay(totalTrades);
}
