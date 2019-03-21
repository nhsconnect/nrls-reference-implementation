import { BaseGenericSystem } from "../BaseGenericSystem";
import { IRequest } from "../../core/interfaces/IRequest";
import * as moment from 'moment';
import { observable } from "aurelia-framework";

export class GsASClinician extends BaseGenericSystem {
    data: any = {};

    callStart?: Date;
    callArrive?: Date;
    callType: string = "EMERG";
    callDesc: string = "EMERG";

    request?: IRequest;

    activate(model) {
        this.data = model;
    }

    startNewCall() {
        this.callStart = new Date();
        this.callArrive = moment().add(7, "minutes").toDate();
    }

    startView() {
        this.toggleInstructions();

        this.trackView(this.data.genericSystem.fModule, this.data.personnel.name);
    }

    private patientChanged(newValue: string, oldValue: string): void {

        if (!newValue) {
            return;
        }

        this.createRequest();
        this.startNewCall();
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

    deactivate() {
        this.data = {};
    }
}


