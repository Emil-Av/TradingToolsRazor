// set a java script event for each dropdown menu in _StatisticsMenuButtons.cshtml when a new item has been changed

(function () {
    function initDropdown({ menuId, spanId }) {
        const menu = document.getElementById(menuId);
        const span = document.getElementById(spanId);
        if (!menu || !span) return;

        menu.addEventListener('click', function (e) {

            var isNewLoad = false;
            const item = e.target;
            if (!item || !item.classList.contains('dropdown-item')) return;

            const selectedText = item.textContent.trim();
            if (!selectedText) return;

            // Update the visible span with user-friendly text
            span.textContent = selectedText;

            if (menuId === 'dropdownBtnTimeFrame' || menuId === 'dropdownBtnStrategy' || menuId === 'dropdownBtnTradeType') {
                isNewLoad = true;
            }

            // Update visual selection in the dropdown
            Array.from(menu.querySelectorAll('.dropdown-item')).forEach(a => a.classList.remove('bg-gray-400'));
            item.classList.add('bg-gray-400');

            loadStatistics(isNewLoad);
        });
    }

    async function loadStatistics(isNewLoad) {
        const timeFrameText = (document.getElementById('spanTimeFrame')?.textContent || '').trim();
        const strategyText = (document.getElementById('spanStrategy')?.textContent || '').trim();
        const tradeTypeText = (document.getElementById('spanTradeType')?.textContent || '').trim();
        const sampleSizeText = (document.getElementById('spanSampleSize')?.textContent || '').trim();
        const timeText = (document.getElementById('spanTime')?.textContent || '').trim();

        const payload = {
            timeFrame: toEnumName('timeFrame', timeFrameText),
            strategy: toEnumName('strategy', strategyText),
            tradeType: toEnumName('tradeType', tradeTypeText),
            sampleSizeNumber: sampleSizeText.toLowerCase() === 'all' ? 0 : parseInt(sampleSizeText, 10),
            time: timeText,
            isNewLoad: isNewLoad
        };

        const params = new URLSearchParams();
        params.set('timeFrame', payload.timeFrame);
        params.set('strategy', payload.strategy);
        params.set('tradeType', payload.tradeType);
        params.set('time', payload.time);
        params.set('sampleSizeNumber', payload.sampleSizeNumber);
        params.set('isNewLoad', payload.isNewLoad);

        // Navigate to GET /Statistics with query to trigger server-side render
        window.location.href = `/Statistics?${params.toString()}`;
    }

    document.addEventListener('DOMContentLoaded', function () {
        initDropdown({ menuId: 'dropdownBtnTimeFrame', spanId: 'spanTimeFrame' });
        initDropdown({ menuId: 'dropdownBtnStrategy', spanId: 'spanStrategy' });
        initDropdown({ menuId: 'dropdownBtnTradeType', spanId: 'spanTradeType' });
        initDropdown({ menuId: 'dropdownBtnOrderType', spanId: 'spanTime' });
        initDropdown({ menuId: 'dropdownBtnSampleSize', spanId: 'spanSampleSize' });
    });
})();