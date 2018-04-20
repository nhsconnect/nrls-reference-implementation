import { bindable, bindingMode } from 'aurelia-framework';

export class DContentModalCustomElement {

    @bindable({ defaultBindingMode: bindingMode.twoWay })
    showActionModal: boolean;

    @bindable
    contentFileUrl: string;

    @bindable
    title: string;
    
    attached() {

        $('.nrls-d-content-modal').modal('show');

        $('.nrls-d-content-modal').on('hidden.bs.modal', () => {

            this.showActionModal = false;
        });
    }

    detached() {

        $('.nrls-d-content-modal').modal('dispose');

    }
}