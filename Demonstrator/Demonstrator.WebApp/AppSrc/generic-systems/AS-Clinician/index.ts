import { BaseGenericSystem } from "../BaseGenericSystem";

export class GsASClinician extends BaseGenericSystem {
    data: any = {};

    activate(model) {
        this.data = model;
    }
}


