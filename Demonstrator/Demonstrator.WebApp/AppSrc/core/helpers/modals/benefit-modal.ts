import { bindable, bindingMode, inject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

@inject(Router)
export class BenefitModalCustomElement {

    @bindable({ defaultBindingMode: bindingMode.oneWay }) benefitforid: string;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) benefitfor: string;
    navigateToAbout: boolean = false;

    constructor(private router: Router) { }

    attached() {
        $('.nrls-benefit-modal').on('hidden.bs.modal', () => {

            if (this.navigateToAbout) {
                this.router.navigateToRoute('about');
            }
        })
    }

    loadBenefitModal() {

        $('.nrls-benefit-modal').modal('show');
    }


    goToAbout() {
        this.navigateToAbout = true;
        $('.nrls-benefit-modal').modal('hide');
    }

    detached() {
        $('.nrls-benefit-modal').modal('dispose');
    }
}