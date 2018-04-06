import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IPatientNumber } from '../interfaces/IPatientNumber';
import { IPatient } from '../interfaces/IPatient';
import { ICarePlan } from '../interfaces/ICarePlan';
import { IRequest } from '../interfaces/IRequest';

@inject(WebAPI)
export class EprSvc {

    baseUrl: string = 'Epr';

    constructor(private api: WebAPI) { }

    /**
    * Requests Crisis Plan by patient NHS Number from the backend service.
    * @returns A Crisis Plan in the form of ICarePlan or null.
    */
    getPatientCrisisPlan(nhsNumbr: string) {
        let crisiPlan = this.api.do<ICarePlan>(`${this.baseUrl}/CrisisPlan/Patient/${nhsNumbr}`, null, 'get');
        return crisiPlan;
    }

    /**
    * Create new Crisis Plan to the backend service.
    * @returns A Crisis Plan in the form of ICarePlan or null.
    */
    create(request: IRequest) {
        let crisiPlan = this.api.do<ICarePlan>(`${this.baseUrl}/CrisisPlan`, request.resource, 'post', request.headers);
        return crisiPlan;
    }

    /**
    * Update Crisis Plan to the backend service.
    * @returns A Crisis Plan in the form of ICarePlan or null.
    */
    update(request: IRequest) {
        let crisiPlan = this.api.do<ICarePlan>(`${this.baseUrl}/CrisisPlan/${request.id}`, request.resource, 'put', request.headers);
        return crisiPlan;
    }

    /**
    * Delete Crisis Plan in the backend service.
    * @returns A Crisis Plan in the form of ICarePlan or null.
    */
    delete(request: IRequest) {
        let crisiPlan = this.api.do<boolean>(`${this.baseUrl}/CrisisPlan/${request.id}`, null, 'delete', request.headers);
        return crisiPlan;
    }

}