import { WebAPI }           from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IBenefitDialog }   from '../interfaces/IBenefitDialog';
import { IBenefitMenu } from '../interfaces/IBenefitMenu';

@inject(WebAPI)
export class BenefitsSvc {

    baseUrl: string = 'Benefits';

    constructor(private api: WebAPI) { }

    /**
     * Requests all Benefits for a type (org or person) with the given id from the backend service.
     * @param forType A valid type (org or person).
     * @param forTypeId The ID of the type (org or person).
     * @returns A benefit dialog model containing a list of benefits for the type requested.
     */
    getFor(forType: string, forTypeId: string) {
        let benefits = this.api.do<IBenefitDialog>(`${this.baseUrl}/${forType}/${forTypeId}`, null, 'get');
        return benefits;
    }

    hasFor(forType: string, forTypeId: string) {
        let hasBenefits = this.api.do<boolean>(`${this.baseUrl}/Has/${forType}/${forTypeId}`, null, 'get');
        return hasBenefits;
    }

    getMenu() {
        let menu = this.api.do<IBenefitMenu>(`${this.baseUrl}/Menu`, null, 'get');
        return menu;
    }

}