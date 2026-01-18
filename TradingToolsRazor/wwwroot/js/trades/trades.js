// Summernote sometimes opens with the cursor in the middle, then that causes errors. Check Review

/**
 * Main trades.js file that loads all modular components
 * 
 * This file dynamically loads all the modular JavaScript files in the correct order:
 * - trades_globals.js: Global variables and constants
 * - trades_helpers.js: Helper/utility functions
 * - trades_tradeNavigation.js: Trade navigation (prev/next) functions
 * - trades_api.js: API calls and AJAX functions
 * - trades_dataLoading.js: Data loading functions
 * - trades_menuManagement.js: Menu management functions
 * - trades_cardMenu.js: Card menu content display functions
 * - trades_editor.js: Summernote editor functions
 * - trades_eventHandlers.js: Event handlers for buttons and UI interactions
 * - trades_initialization.js: Initialization functions
 * - trades_main.js: Main initialization call
 */

(function() {
    'use strict';

    // Array of script files to load in order
    const scripts = [
        '/js/trades/trades_globals.js',
        '/js/trades/trades_helpers.js',
        '/js/trades/trades_tradeNavigation.js',
        '/js/trades/trades_api.js',
        '/js/trades/trades_dataLoading.js',
        '/js/trades/trades_menuManagement.js',
        '/js/trades/trades_cardMenu.js',
        '/js/trades/trades_editor.js',
        '/js/trades/trades_eventHandlers.js',
        '/js/trades/trades_initialization.js',
        '/js/trades/trades_main.js'
    ];

    // Function to load a script dynamically
    function loadScript(src) {
        return new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = src;
            script.async = false; // Ensure scripts load in order
            script.onload = () => resolve(src);
            script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
            document.head.appendChild(script);
        });
    }

    // Load all scripts sequentially
    async function loadAllScripts() {
        try {
            for (const scriptSrc of scripts) {
                await loadScript(scriptSrc);
            }
            console.log('All trades modules loaded successfully');
        } catch (error) {
            console.error('Error loading trades modules:', error);
        }
    }

    // Start loading scripts
    loadAllScripts();
})();

