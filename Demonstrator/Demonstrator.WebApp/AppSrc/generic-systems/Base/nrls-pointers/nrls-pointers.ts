import { bindable, inject } from 'aurelia-framework';
import { PointerSvc } from '../../../core/services/PointerService';
import { IPointer } from '../../../core/interfaces/IPointer';

@inject(PointerSvc)
export class NrlsPointers {

    pointers: Array<IPointer> = [];
    pointersLoading: boolean = false;
    patientId?: string = undefined;

    constructor(private pointerSvc: PointerSvc) { }

    activate(patientId: string) {
        this.patientId = patientId;
    }

    attached() {
        this.getPointers();
    }

    getPointers() {
        if (!this.patientId) {
            return;
        }

        this.pointersLoading = true;
        this.pointerSvc.getPointers(this.patientId).then(pointers => {
            this.pointers = pointers;
            this.pointersLoading = false;
        });
    }
}