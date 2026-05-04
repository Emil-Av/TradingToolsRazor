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
    $('#ButtonEdit').addClass('d-none');
    $('#ButtonUpdate').removeClass('d-none');
    $('#ButtonUpload').removeClass('d-none');
    $('#ButtonDelete').removeClass('d-none');
}

function showJournalContent() {
    currentTabSelector = '#pre';
    // Reset all journal tab panes first
    $('#journalTabContent .tab-pane').removeClass('show active');
    // Activate the Pre tab pane
    $('#pre').addClass('show active');
    // Reset and activate the Pre tab button
    $('#journalTabHeaders .nav-link').removeClass('active').attr('aria-selected', 'false');
    $('#pre-tab').addClass('active').attr('aria-selected', 'true');

    $('#journalTabHeaders').removeClass('d-none');
    $('#journalTabContent').removeClass('d-none');
    $('#reviewTabHeaders').addClass('d-none');
    $('#reviewTabContent').addClass('d-none');
    $('#tradeDataTabContent').addClass('d-none');
    $('#researchTabContent').addClass('d-none');
    $('#ButtonEdit').removeClass('d-none');
    $('#ButtonUpdate').addClass('d-none');
    $('#ButtonUpload').addClass('d-none');
    $('#ButtonDelete').addClass('d-none');
}

function showResearchContent() {
    $('#researchTabContent').removeClass('d-none');
    $('#tradeDataTabContent').addClass('d-none');
    $('#journalTabHeaders').addClass('d-none');
    $('#journalTabContent').addClass('d-none');
    $('#reviewTabHeaders').addClass('d-none');
    $('#reviewTabContent').addClass('d-none');
    $('#ButtonEdit').addClass('d-none');
    $('#ButtonUpdate').removeClass('d-none');
    $('#ButtonUpload').removeClass('d-none');
    $('#ButtonDelete').removeClass('d-none');
}

function showReviewContent() {
    currentTabSelector = '#first';
    // Reset all review tab panes first
    $('#reviewTabContent .tab-pane').removeClass('show active');
    // Activate the First tab pane
    $('#first').addClass('show active');
    // Reset and activate the First tab button
    $('#reviewTabHeaders .nav-link').removeClass('active').attr('aria-selected', 'false');
    $('#first-tab').addClass('active').attr('aria-selected', 'true');

    $('#reviewTabHeaders').removeClass('d-none');
    $('#reviewTabContent').removeClass('d-none');
    $('#journalTabHeaders').addClass('d-none');
    $('#journalTabContent').addClass('d-none');
    $('#tradeDataTabContent').addClass('d-none');
    $('#researchTabContent').addClass('d-none');
    $('#ButtonEdit').removeClass('d-none');
    $('#ButtonUpdate').addClass('d-none');
    $('#ButtonUpload').addClass('d-none');
    $('#ButtonDelete').addClass('d-none');
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
