import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IPointer } from '../interfaces/IPointer';

@inject(WebAPI)
export class PointerSvc {

    baseUrl: string = 'Nrls';

    constructor(private api: WebAPI) { }

    /**
     * Requests all Pointers for a Patient from the backend service.
     * @param nhsNumber A valid patient NHS Number without dashes.
     * @returns A list of FHIR DocumentReferences in the form of IPointer.
     */
    getPointers(id: string) {
        let pointers = this.api.do<Array<IPointer>>(`${this.baseUrl}/${id}`, null, 'get');
        return pointers;
    }

}