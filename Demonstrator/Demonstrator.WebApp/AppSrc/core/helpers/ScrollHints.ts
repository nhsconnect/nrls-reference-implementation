export class ScrollHints {
    
    constructor() {
        $('.btn-nrls-hint').show();
    }

    public scroll(elementId: string) {

        let elm = $(elementId);
        let scrollPos = elm.offset();

        if (scrollPos !== undefined) {
            let middleWindow = ($(window).outerHeight() || 0) / 2;
            $('html, body').animate({ scrollTop: (scrollPos.top - middleWindow) }, 1250);
        }      
    }

    public hide(elementId: string) {

        let src = $('[data-showhints="' + elementId + '"]');
        let elm = $(elementId);

        let elmPos = elm.offset();
        let scrollPos = $(window).scrollTop();

        if (elmPos && scrollPos) {

            let bottomWindow = ($(window).outerHeight() || 0);
            let hideAt = (elmPos.top - bottomWindow) + 150;

            if (scrollPos > hideAt) {
                $(src).hide();
            } else {
                $(src).show();
            }
        }
    }
}