import { BaseGenericSystem } from "../BaseGenericSystem";
import { IRequest } from "../../core/interfaces/IRequest";
import * as moment from 'moment';

export class GsASClinician extends BaseGenericSystem {
    data: any = {};

    pdsActive: boolean = false;
    activeCall: boolean = false;
    callStart?: Date;
    callArrive?: Date;
    callType?: string;
    callDesc?: string;

    request?: IRequest;

    activate(model) {
        this.data = model;
    }

    startNewCall() {
        this.activeCall = true;
        this.callStart = new Date();
        this.callArrive = moment().add(7, "minutes").toDate();
        this.callType = "EMERG";
        this.callDesc = "EMERG";
        this.patient = undefined;

        this.findPatient();
    }

    endCall() {
        this.activeCall = false;
        this.callStart = 
        this.callArrive = 
        this.callType = 
        this.callDesc = 
        this.patient = 
        this.selectedPatient =
        this.request = undefined;
    }

    setPdsStatus() {
        return this.pdsActive = !this.pdsActive;
    }

    private patientChanged(newValue: string, oldValue: string): void {

        if (!newValue) {
            return;
        }

        this.createRequest();
    }

    private createRequest(): void {
        this.request = <IRequest>{
            headers: {
                "Asid": this.data.genericSystem.asid,
                "OrgCode": this.data.organisation.orgCode
            },
            id: this.patient ? this.patient.nhsNumber : null,
            active: true
        };
    }
}


