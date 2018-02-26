import { PatientSvc } from '../core/services/PatientService';
import { bindable, inject } from 'aurelia-framework';
import { IPatientNumber } from "../core/models/IPatientNumber";

@inject(PatientSvc)
export class BaseGenericSystem {
    nhsNumbers: Array<IPatientNumber> = [];
    nhsNumbersLoading: boolean = false;

    constructor(private patientSvc: PatientSvc) { }

    created() {

        this.nhsNumbersLoading = true;
        this.patientSvc.getPatientNumbers().then(numbers => {
            this.nhsNumbers = numbers;
            this.nhsNumbersLoading = false;
        });
    }

    getPatient(nhsNumber: number) {
        this.patientSvc.getPatient(nhsNumber).then(patient => console.log(patient));
    }
}