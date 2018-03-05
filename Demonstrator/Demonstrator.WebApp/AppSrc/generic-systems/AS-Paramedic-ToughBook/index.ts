import { BaseGenericSystem } from "../BaseGenericSystem";

export class GsASParamedicToughBook extends BaseGenericSystem {
    data: any = {};

    activate(model) {
        this.data = model;
    }
}


