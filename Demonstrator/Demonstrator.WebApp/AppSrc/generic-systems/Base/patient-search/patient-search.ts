import { bindable, inject, bindingMode }    from 'aurelia-framework';
import { PatientSvc }                       from '../../../core/services/PatientService';
import { IPatientNumber }                   from '../../../core/interfaces/IPatientNumber';
import { IPatient }                         from '../../../core/interfaces/IPatient';

@inject(PatientSvc)
export class PatientSearch {

    nhsNumbers: Array<IPatientNumber> = [];
    nhsNumbersLoading: boolean = false;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) patientLoading: boolean;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) selectedPatient?: string;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) patientDetails: IPatient;
    currentPatient?: string;

    constructor(private patientSvc: PatientSvc) { }

    attached() {
        this.nhsNumbersLoading = true;
        this.patientSvc.getPatientNumbers().then(numbers => {
            this.nhsNumbers = numbers;
            this.nhsNumbersLoading = false;
        });
    }

    setPatient() {
        //this.patientLoading = true;
        this.selectedPatient = this.currentPatient;
        this.currentPatient = undefined;
    }

    detached() {
        //this.patientLoading = false;
    }

}