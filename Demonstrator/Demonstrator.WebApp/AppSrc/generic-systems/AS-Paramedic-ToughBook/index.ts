import { BaseGenericSystem } from "../BaseGenericSystem";
import { IRequest } from "../../core/interfaces/IRequest";

export class GsASParamedicToughBook extends BaseGenericSystem {
    data: any = {};

    request: IRequest;

    activate(model) {
        this.data = model;

        this.createRequest();
    }

    private createRequest(): void {

        this.request = <IRequest>{
            headers: {
                "Asid": this.data.genericSystem.asid,
                "OrgCode": this.data.organisation.orgCode
            },
            id: this.patient ? this.patient.nhsNumber : null
        };
    }


}


