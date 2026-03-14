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
    if ($('#ButtonEdit').hasClass('d-none')) {
        $('#ButtonEdit').removeClass('d-none');
        $('.editorOnBtns').addClass('d-none');
    } else {
        $('#ButtonEdit').addClass('d-none');
        $('.editorOnBtns').removeClass('d-none');
    }
}
