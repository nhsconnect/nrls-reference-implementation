import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IPatientNumber } from '../models/IPatientNumber';
import { IPatient } from '../models/IPatient';

@inject(WebAPI)
export class PatientSvc {

    baseUrl: string = 'Patients';

    constructor(private api: WebAPI) { }

    /**
    * Requests all available patient NHS Numbers from the backend service.
    * @returns A list of available NHS Numbers in the form of IPatientNumber.
    */
    getPatientNumbers() {
        let personnel = this.api.do<Array<IPatientNumber>>(`${this.baseUrl}/Numbers`, null, 'get');
        return personnel;
    }

    /**
     * Requests a Patient Resource from the backend service.
     * @param nhsNumber A valid patient NHS Number without dashes.
     * @returns A single FHIR Patient in the form of IPatient.
     */
    getPatient(nhsNumber: number) {
        let personnel = this.api.do<IPatient>(`${this.baseUrl}/${nhsNumber}`, null, 'get');
        return personnel;
    }

}