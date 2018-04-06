import { bindable, bindingMode } from 'aurelia-framework';

export class ScenarioModalCustomElement {

    @bindable({ defaultBindingMode: bindingMode.oneWay }) cmodule: string;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) title: string;

    loadScenarioModal() {

        $('.nrls-scenario-modal').modal('show');
    }
}