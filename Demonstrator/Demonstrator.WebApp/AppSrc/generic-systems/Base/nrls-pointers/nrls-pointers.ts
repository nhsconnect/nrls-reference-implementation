﻿import { bindable, inject, observable, bindingMode } from 'aurelia-framework';
import { PointerSvc } from '../../../core/services/PointerService';
import { IPointer } from '../../../core/interfaces/IPointer';
import { IRequest } from '../../../core/interfaces/IRequest';
import { IDocumentRequest } from '../../../core/interfaces/IDocumentRequest';
import { IPointerDocument } from '../../../core/interfaces/IPointerDocument';

@inject(PointerSvc)
export class NrlsPointers {

    pointers: Array<IPointer> = [];
    document: IDocumentRequest = { headers: {} };

    @bindable({ defaultBindingMode: bindingMode.twoWay })
    pointerDocument?: IPointerDocument;

    @bindable({ defaultBindingMode: bindingMode.twoWay })
    pointerDocumentLoaded?: boolean;

    pointersLoading: boolean = false;

    @observable
    @bindable({ defaultBindingMode: bindingMode.oneWay })
    request?: IRequest;

    constructor(private pointerSvc: PointerSvc) { }

    private requestChanged(newValue: IRequest, oldValue: IRequest): void {
        if (newValue && newValue.active) {
            this.getPointers();
        }
      
    }

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

    getDocument(pointerId, nhsNumber) {

        this.document.headers = this.request != null ? this.request.headers : {};
        this.document.id = pointerId;
        this.document.nhsNumber = nhsNumber;

        this.pointerDocument = {
            url: undefined,
            document: null,
            currentPage: 1,
            totalPages: 1,
            scale: 1,
            loading: true
        };

        this.pointerDocumentLoaded = false;

        // TODO: cache response
        this.pointerSvc.getPointerDocument(this.document).then(binary => {

            if (this.pointerDocument) {
                this.pointerDocument.url = `data:application/pdf;base64,${binary.contentElement.value}`;
                this.pointerDocument.loading = false;

                this.pointerDocumentLoaded = true;
            }

        });
    }

}