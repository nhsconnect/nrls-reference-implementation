import { PatientSvc } from '../core/services/PatientService';
import { bindable, inject, observable } from 'aurelia-framework';
import { IPatientNumber } from "../core/interfaces/IPatientNumber";
import { IPatient } from '../core/interfaces/IPatient';
import { ValidationControllerFactory, validateTrigger } from 'aurelia-validation';
import { EprSvc } from '../core/services/EprService';

@inject(PatientSvc, EprSvc, ValidationControllerFactory)
export class BaseGenericSystem {

    vdController: any;

    @observable
    protected patient?: IPatient;

    patientLoading: boolean = false;
    instructionsActive: boolean = true;

    @observable
    selectedPatient?: IPatientNumber;

    constructor(private patientSvc: PatientSvc, protected eprSvc: EprSvc, private validationControllerFactory: ValidationControllerFactory) {
        this.vdController = validationControllerFactory.createForCurrentScope();
        this.vdController.validateTrigger = validateTrigger.manual;
    }

    attached() {

        $('[data-toggle="tooltip"]').tooltip();
    }

    toggleInstructions() {
        this.instructionsActive = !this.instructionsActive;
    }

    getPatient() {
        if (!this.selectedPatient || !this.selectedPatient.nhsNumber || this.selectedPatient.id.length === 0) {
            return;
        }

        this.patientLoading = true;
        this.patientSvc.getPatient(`${this.selectedPatient.nhsNumber}`).then(patient => {
            this.patient = patient;
            this.patientLoading = false;
        });
    }

    findPatient() {
        this.patient = undefined;
    }

    private selectedPatientChanged(newValue: string, oldValue: string): void {

        if (newValue === oldValue) {
            return;
        }

        this.getPatient();
    }

    get showPatientSearch(): boolean {

        return !this.patient;
    }

    detached() {

        $('[data-toggle="tooltip"]').tooltip('dispose');
    }

}