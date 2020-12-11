import { bindable, inject, bindingMode } from "aurelia-framework";
import { PatientSvc } from "../../../core/services/PatientService";
import { IPatientNumber } from "../../../core/interfaces/IPatientNumber";
import { IPatient } from "../../../core/interfaces/IPatient";
import { IPointer } from "../../../core/interfaces/IPointer";

@inject(PatientSvc)
export class PatientSearch {
  nhsNumbers: Array<IPatientNumber> = [];

  nhsNumbersLoading: boolean = false;

  @bindable({ defaultBindingMode: bindingMode.oneWay })
  patientLoading: boolean;

  @bindable({ defaultBindingMode: bindingMode.twoWay })
  selectedPatient?: IPatientNumber;

  @bindable({ defaultBindingMode: bindingMode.oneWay })
  patientDetails: IPatient;

  @bindable({ defaultBindingMode: bindingMode.oneWay })
  setCurrent: boolean;

  currentPatient?: IPatientNumber;

  @bindable
  type?: string;

  constructor(private patientSvc: PatientSvc) {}

  attached() {
    this.nhsNumbersLoading = true;
    this.patientSvc.getPatientNumbers().then((numbers) => {
      this.nhsNumbers = numbers;
      this.nhsNumbersLoading = false;

      if (this.setCurrent && this.nhsNumbers && this.nhsNumbers.length > 0) {
        this.selectedPatient = this.nhsNumbers[0];
      }
    });
  }

  setPatient() {
    this.selectedPatient = this.currentPatient;
    this.currentPatient = undefined;
  }

  detached() {
    this.type = this.currentPatient = this.selectedPatient = undefined;
    this.nhsNumbers = [];
  }
}
