import { bindable, bindingMode, inject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

@inject(Router)
export class ScenarioModalCustomElement {

    constructor(private router: Router) { }

    @bindable({ defaultBindingMode: bindingMode.oneWay })
    cmodule: string;

    @bindable({ defaultBindingMode: bindingMode.oneWay })
    title: string;

    navigateToBenefits: boolean = false;

    loadScenarioModal() {

        $('.nrls-scenario-modal').modal('show');

        $('.nrls-scenario-modal').on('hidden.bs.modal', () => {
            if (this.navigateToBenefits) {
                this.router.navigateToRoute('about-benefits');
                this.navigateToBenefits = false;
            }
        });
    }

    setBenefitsNav() {
        this.navigateToBenefits = true;
        $('.nrls-scenario-modal').modal('hide');
    }
}