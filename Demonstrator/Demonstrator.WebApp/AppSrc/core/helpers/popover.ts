import { customAttribute, bindable, inject } from 'aurelia-framework';

@inject(Element)
@customAttribute('popover')
export class Popover {

    @bindable title;
    @bindable content;
    @bindable placement;

    constructor(public element: Element) {}

    bind() {

        let popoverData : string = $(this.element).prop("popoverData") || "";
        let data = {};
        try {
            data = JSON.parse(popoverData);
        } catch (e) {
            //if its bad just ignore
        }

        for (var key in data)
        {
            if (data.hasOwnProperty(key)) {
                this.content = this.content.replace("{" + key + "}", data[key])
            }
        }

        $(this.element).popover({
            title: this.title,
            placement: this.placement,
            content: this.content,
            trigger: 'focus',
            container: 'body'
        });

    }

}