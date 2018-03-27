import { bindable, inject } from 'aurelia-framework';
import { PointerSvc } from '../../../core/services/PointerService';
import { IPointer } from '../../../core/interfaces/IPointer';

@inject(PointerSvc)
export class NrlsPointers {

    pointers: Array<IPointer> = [];
    pointersLoading: boolean = false;
    patientNhsNumber?: number = undefined;

    constructor(private pointerSvc: PointerSvc) { }

    activate(patientNhsNumber: number) {
        this.patientNhsNumber = patientNhsNumber;
    }

    attached() {
        this.getPointers();
    }

    getPointers() {
        if (!this.patientNhsNumber) {
            return;
        }

        this.pointersLoading = true;
        this.pointerSvc.getPointers(this.patientNhsNumber).then(pointers => {
            this.pointers = pointers;
            this.pointersLoading = false;
        });
    }
}