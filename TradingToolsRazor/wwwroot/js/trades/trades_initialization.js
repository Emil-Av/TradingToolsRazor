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
    $('#btnUpdate').on('click', handleUpdateClick);
    $('#btnDelete').on('click', handleDeleteClick);
    $('#btnEdit').on('click', openEditor);
    $('#btnSave').on('click', saveEditorText);
    $('#btnCancel').on('click', handleCancelClick);

    // Card menu events
    $('#headerMenu').on('click', '.dropdown-item', handleCardMenuClick);

    // Trade navigation events
    $('#btnNext').on('click', handleNextClick);
    $('#btnPrev').on('click', handlePrevClick);
    $('#tradeNumberInput').on('keypress', handleTradeNumberInput);
    $(document).on('keydown', handleKeyboardNavigation);
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
