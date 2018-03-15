import { bindable, bindingMode } from 'aurelia-framework';

export class ErrorModalCustomElement {

    @bindable dialog: any;

    attached() {

        $('.nrls-error-modal').modal('show');

        $('.nrls-error-modal').on('hidden.bs.modal', (e) => {
            this.dialog = null;
        })
        
    }
}