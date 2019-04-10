import { BaseGenericSystem } from "../BaseGenericSystem";
import { IRequest } from "../../core/interfaces/IRequest";
import * as moment from 'moment';
import { observable } from "aurelia-framework";

export class GsASSCRa extends BaseGenericSystem {
    data: any = {};

    showSearch: boolean = true;
    showDetail: boolean = false;

    request?: IRequest;

    activate(model) {
        this.data = model;
    }

    findPatient() {
        this.showSearch = true;
        this.showDetail = false;

        this.patient = undefined;
    }

    displayDetail() {
        this.showSearch = false;
        this.showDetail = true;
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
        this.displayDetail();
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


