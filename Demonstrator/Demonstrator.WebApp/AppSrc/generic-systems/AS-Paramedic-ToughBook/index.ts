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

    showHome() {
        this.nrlsTabActive = false;
        this.pageTitle = "Electronic Patient Record";
    }

    showNrls() {
        this.nrlsTabActive = true;
        this.pageTitle = "Electronic Patient Record : NRLS";
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


