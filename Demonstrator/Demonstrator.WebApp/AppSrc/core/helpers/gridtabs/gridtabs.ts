import { bindable, bindingMode } from 'aurelia-framework';

export class gridtabs {
    @bindable({ defaultBindingMode: bindingMode.oneWay }) items;
    @bindable route: string;
    @bindable icon: string;
    @bindable showMoreHint: boolean;
    @bindable showMoreTitle: string;
    @bindable showMoreBody: string;

    created() {

        $(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
    }

}