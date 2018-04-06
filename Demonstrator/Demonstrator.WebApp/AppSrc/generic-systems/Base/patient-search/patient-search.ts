import { bindable, inject, bindingMode }    from 'aurelia-framework';
import { PatientSvc }                       from '../../../core/services/PatientService';
import { IPatientNumber }                   from '../../../core/interfaces/IPatientNumber';
import { IPatient }                         from '../../../core/interfaces/IPatient';

@inject(PatientSvc)
export class PatientSearch {

    nhsNumbers: Array<IPatientNumber> = [];
    nhsNumbersLoading: boolean = false;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) patientLoading: boolean;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) selectedPatient?: IPatientNumber;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) patientDetails: IPatient;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) setCurrent: boolean;
    currentPatient?: IPatientNumber;

    constructor(private patientSvc: PatientSvc) { }

    attached() {
        this.nhsNumbersLoading = true;
        this.patientSvc.getPatientNumbers().then(numbers => {
            this.nhsNumbers = numbers;
            this.nhsNumbersLoading = false;

            if (this.setCurrent && this.nhsNumbers && this.nhsNumbers.length > 0) {
                this.updateSelected(this.nhsNumbers[0]);
            }
        });
    }

    updateSelected(selected: IPatientNumber) {
        this.currentPatient = selected;
        this.setPatient();
    }

    setPatient() {
        this.selectedPatient = this.currentPatient;
    }

}