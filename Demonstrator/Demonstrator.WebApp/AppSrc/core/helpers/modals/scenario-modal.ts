import { bindable, bindingMode } from 'aurelia-framework';

export class ScenarioModalCustomElement {

    @bindable({ defaultBindingMode: bindingMode.oneWay })
    cmodule: string;

    @bindable({ defaultBindingMode: bindingMode.oneWay })
    title: string;

    switchContext: boolean = false;

    switchTimer: any;

    switchAnimation: boolean = true;

    toggleContext() {
        this.switchContext = !this.switchContext;
    }

    toggleAnimation() {
        this.switchAnimation = !this.switchAnimation;

        if (!this.switchAnimation) {
            this.stopContext();
        }
        else {
            this.animateContext();
        }
    }

    animateContext() {
        this.switchTimer = window.setInterval(() => this.toggleContext(), 2500);
    }

    initContext() {
        let ani = window.setTimeout(() => {
            this.toggleContext();
            this.animateContext();
        }, 1000);
    }

    stopContext() {
        window.clearInterval(this.switchTimer);
    }

    loadScenarioModal() {

        $('.nrls-scenario-modal').modal('show');

        $('.nrls-scenario-modal').on('shown.bs.modal', () => {
            this.initContext();
        });

        $('.nrls-scenario-modal').on('hide.bs.modal', () => {
            this.stopContext();
        })
    }
}