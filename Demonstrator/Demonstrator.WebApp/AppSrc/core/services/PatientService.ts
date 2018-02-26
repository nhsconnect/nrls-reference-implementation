import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IPatientNumber } from '../models/IPatientNumber';
import { IPatient } from '../models/IPatient';

@inject(WebAPI)
export class PatientSvc {

    baseUrl: string = 'Patients';

    constructor(private api: WebAPI) { }

    getPatientNumbers() {
        let personnel = this.api.do<Array<IPatientNumber>>(`${this.baseUrl}/Numbers`, null, 'get');
        return personnel;
    }

    getPatient(nhsNumber: number) {
        let personnel = this.api.do<IPatient>(`${this.baseUrl}/${nhsNumber}`, null, 'get');
        return personnel;
    }

}