export class ScrollHints {

    constructor() {
        $('.btn-nrls-hint').show();
    }

    public scroll(elementId: string) {

        let elm = $(elementId);
        let scrollPos = elm.offset();

        if (scrollPos) {
            $('document').scrollTop(scrollPos.top);
        }      
    }

    public hide(src: Element, elementId: string) {

        let elm = $(elementId);
        let elmPos = elm.offset();
        let scrollPos = $('window').scrollTop();

        if (elmPos)
            console.log(elmPos.top, scrollPos);

        if (scrollPos && elmPos &&
            scrollPos > elmPos.top) {
                $(src).hide();
        }

    }
}