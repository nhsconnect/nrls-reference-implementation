import { customAttribute, bindable, inject } from 'aurelia-framework';

@inject(Element)
@customAttribute('popover')
export class Popover {

    @bindable title;
    @bindable content;
    @bindable placement;

    constructor(public element: Element) {}

    bind() {
        $(this.element).popover({
            title: this.title,
            placement: this.placement,
            content: this.content,
            trigger: 'focus',
            container: 'body'
        });

    }

}