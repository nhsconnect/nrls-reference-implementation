import { bindable, bindingMode } from 'aurelia-framework';

export class ContactModalCustomElement {

    @bindable({ defaultBindingMode: bindingMode.oneWay })
    canShowContact?: boolean;

    @bindable
    dialog: any;

    private canShowContactChanged(newValue: boolean, oldValue: boolean): void {

        if (newValue && newValue === true) {
            this.showContactModal();
        }

    }

    private showContactModal() {

        $('.nrls-contact-modal').modal('show');

        $('.nrls-contact-modal').on('hidden.bs.modal', (e) => {
            this.dialog = null;
            this.canShowContact = false;
        })
        
    }
}