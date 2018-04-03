import { bindable, inject, bindingMode }    from 'aurelia-framework';
import { PatientSvc }                       from '../../../core/services/PatientService';
import { IPatientNumber }                   from '../../../core/interfaces/IPatientNumber';

@inject(PatientSvc)
export class PatientSearch {

    nhsNumbers: Array<IPatientNumber> = [];
    nhsNumbersLoading: boolean = false;
    patientLoading: boolean = false;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) selectedPatient: string;
    currentPatient: string;

    constructor(private patientSvc: PatientSvc) { }

    attached() {
        this.nhsNumbersLoading = true;
        this.patientSvc.getPatientNumbers().then(numbers => {
            this.nhsNumbers = numbers;
            this.nhsNumbersLoading = false;
        });
    }

    setPatient() {
        this.patientLoading = true;
        this.selectedPatient = this.currentPatient;
    }

    detached() {
        this.patientLoading = false;
    }

}