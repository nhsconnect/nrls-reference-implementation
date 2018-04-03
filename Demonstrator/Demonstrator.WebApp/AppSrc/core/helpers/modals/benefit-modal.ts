import { bindable, bindingMode } from 'aurelia-framework';

export class BenefitModalCustomElement {

    @bindable({ defaultBindingMode: bindingMode.oneWay }) benefitforid: string;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) benefitfor: string;
    //benefitforid: string;

    attached() {
    }

    loadBenefitModal() {

        $('.nrls-benefit-modal').modal('show');
    }
}