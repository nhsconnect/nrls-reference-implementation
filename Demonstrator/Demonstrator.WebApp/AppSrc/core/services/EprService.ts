import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IPatientNumber } from '../interfaces/IPatientNumber';
import { IPatient } from '../interfaces/IPatient';
import { ICarePlan } from '../interfaces/ICarePlan';

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
    create(crisisPlan: ICarePlan) {
        let crisiPlan = this.api.do<ICarePlan>(`${this.baseUrl}/CrisisPlan`, crisisPlan, 'post');
        return crisiPlan;
    }

    /**
    * Update Crisis Plan to the backend service.
    * @returns A Crisis Plan in the form of ICarePlan or null.
    */
    update(crisisPlan: ICarePlan) {
        let crisiPlan = this.api.do<ICarePlan>(`${this.baseUrl}/CrisisPlan/${crisisPlan.id}`, crisisPlan, 'put');
        return crisiPlan;
    }

    /**
    * Delete Crisis Plan in the backend service.
    * @returns A Crisis Plan in the form of ICarePlan or null.
    */
    delete(planId: string) {
        let crisiPlan = this.api.do<boolean>(`${this.baseUrl}/CrisisPlan/${planId}`, null, 'delete');
        return crisiPlan;
    }

}