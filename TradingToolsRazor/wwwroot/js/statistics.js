// set a java script event for each dropdown menu in _StatisticsMenuButtons.cshtml when a new item has been changed

(function () {
    function initDropdown({ menuId, spanId, paramKey }) {
        const menu = document.getElementById(menuId);
        const span = document.getElementById(spanId);
        if (!menu || !span) return;

        menu.addEventListener('click', function (e) {
            const item = e.target;
            if (!item || !item.classList.contains('dropdown-item')) return;

            const selectedText = item.textContent.trim();
            if (!selectedText) return;

            // Prefer enum value passed from markup, otherwise map from display text
            const selectedEnumValue = (item.dataset && item.dataset.value ? item.dataset.value.trim() : toEnumName(paramKey, selectedText));

            // Update the visible span with user-friendly text
            span.textContent = selectedText;

            // Update visual selection in the dropdown
            Array.from(menu.querySelectorAll('.dropdown-item')).forEach(a => a.classList.remove('bg-gray-400'));
            item.classList.add('bg-gray-400');

            // Call backend API with enum-correct values
            loadStatistics(paramKey, selectedEnumValue);
        });
    }

    function getAntiForgeryToken() {
        const el = document.getElementById('__RequestVerificationToken');
        return el ? el.value : '';
    }

    async function loadStatistics(key, enumValue) {
        // Build payload using enum identifiers so model binding to GetStatisticsModel succeeds
        const payload = {
            timeFrame: toEnumName('timeFrame', (document.getElementById('spanTimeFrame')?.textContent || '').trim()),
            strategy: toEnumName('strategy', (document.getElementById('spanStrategy')?.textContent || '').trim()),
            tradeType: toEnumName('tradeType', (document.getElementById('spanTradeType')?.textContent || '').trim()),
            time: (document.getElementById('spanTime')?.textContent || '').trim() // HH:mm
        };

        // Override changed key
        payload[key] = enumValue;

        const token = getAntiForgeryToken();

        try {
            const res = await fetch('/Statistics?handler=GetStatistics', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                console.error('GetStatistics request failed:', res.status, res.statusText);
                return;
            }

            const data = await res.json();

            if (data.candleBracketingStatsHtml) {
                const container = findStatsContainer();
                if (container) container.innerHTML = data.candleBracketingStatsHtml;
                return;
            }

        } catch (err) {
            console.error('Error calling backend GetStatistics handler:', err);
        }
    }

    function findStatsContainer() {
        const rows = document.querySelectorAll('.row.justify-content-center.align-items-top .container.mb-4 .row.justify-content-center');
        return rows.length ? rows[0] : null;
    }

    document.addEventListener('DOMContentLoaded', function () {
        initDropdown({ menuId: 'dropdownBtnTimeFrame', spanId: 'spanTimeFrame', paramKey: 'timeFrame' });
        initDropdown({ menuId: 'dropdownBtnStrategy', spanId: 'spanStrategy', paramKey: 'strategy' });
        initDropdown({ menuId: 'dropdownBtnTradeType', spanId: 'spanTradeType', paramKey: 'tradeType' });
        initDropdown({ menuId: 'dropdownBtnOrderType', spanId: 'spanTime', paramKey: 'time' });
    });
})();