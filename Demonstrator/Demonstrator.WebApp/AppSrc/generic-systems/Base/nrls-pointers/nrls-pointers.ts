import { bindable, inject, observable } from 'aurelia-framework';
import { PointerSvc } from '../../../core/services/PointerService';
import { IPointer } from '../../../core/interfaces/IPointer';
import { IRequest } from '../../../core/interfaces/IRequest';

@inject(PointerSvc)
export class NrlsPointers {

    pointers: Array<IPointer> = [];
    pointersLoading: boolean = false;

    @observable
    request?: IRequest;

    constructor(private pointerSvc: PointerSvc) { }

    activate(request: IRequest) {
        this.request = request;
    }

    attached() {
        //this.getPointers();
    }

    private requestChanged(newValue: IRequest, oldValue: IRequest): void {

        if (newValue && newValue.active) {
            this.getPointers();
        }
      
    }

    //showPopup(e: JQueryEventObject) {
    //    $(e.currentTarget).popover('toggle');
    //}

    getPointers() {
        if (!this.request || !this.request.id) {
            this.pointers = [];
            return;
        }

        this.pointersLoading = true;
        this.pointerSvc.getPointers(this.request).then(pointers => {
            this.pointers = pointers;
            this.pointersLoading = false;
        });
    }
}