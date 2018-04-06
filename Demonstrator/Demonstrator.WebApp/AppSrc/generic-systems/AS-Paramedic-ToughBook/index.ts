import { BaseGenericSystem } from "../BaseGenericSystem";
import { IRequest } from "../../core/interfaces/IRequest";

export class GsASParamedicToughBook extends BaseGenericSystem {
    data: any = {};
    pageTitle: string = "Electronic Patient Record";
    nrlsTabActive: boolean = false;
    request: IRequest;

    activate(model) {
        this.data = model;
    }

    tabSwitch() {
        this.nrlsTabActive = !this.nrlsTabActive;

        if (this.nrlsTabActive) {
            this.pageTitle = "Electronic Patient Record : NRLS";
        }
        else {
            this.pageTitle = "Electronic Patient Record";
        }
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


