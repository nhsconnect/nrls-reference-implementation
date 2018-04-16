import { bindable, bindingMode } from 'aurelia-framework';

export class ContactModalCustomElement {

    @bindable dialog: any;

    attached() {

        $('.nrls-contact-modal').modal('show');

        $('.nrls-contact-modal').on('hidden.bs.modal', (e) => {
            this.dialog = null;
        })
        
    }
}