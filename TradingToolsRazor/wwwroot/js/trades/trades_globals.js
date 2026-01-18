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

// Trade navigation state (for prev/next navigation)
let tradeIndex = 0;
let lastTradeIndex = 0;
let allTrades = [];
let totalTradesInSampleSize = 0;

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
