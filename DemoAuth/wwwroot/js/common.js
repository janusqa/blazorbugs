window.blazorInterop = {
    Select2: function (selector, url) {
        $(document).ready(function () {
            $(`.${selector}`).select2({
                theme: 'bootstrap-5',
                placeholder: $(this).data('placeholder'),
                minimumInputLength: 1,
                allowClear: true,
                ajax: {
                    delay: 250,
                    url: url,
                    dataType: 'json',
                    cache: false,
                    data: function (params) {
                        return {
                            q: params.term,
                            page: params.page || 0,
                        };
                    },
                    processResults: function (data) {
                        return data.Result;
                    },
                },
            });
        });
    },
    select2ObserverInit: (selector, url) => {
        const observerOptions = { childList: true, subtree: true };
        const select2Parent = document.querySelector(
            '.e-gridcontent tbody[role="rowgroup"]'
        );
        const select2Observer = new MutationObserver(
            (mutationsList, observer) => {
                const targetNode = select2Parent.querySelector(
                    `select.${selector}`
                );
                for (let mutation of mutationsList) {
                    if (mutation.type === 'childList') {
                        if (targetNode) {
                            blazorInterop.Select2(selector, url);
                            observer.disconnect();
                            break;
                        }
                    }
                }
            }
        );
        select2Observer.observe(select2Parent, observerOptions);
    },
};
