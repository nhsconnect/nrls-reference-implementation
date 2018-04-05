import { BaseGenericSystem } from "../BaseGenericSystem";

export class GsASCallHandler extends BaseGenericSystem {
    data: any = {};

    activate(model) {
        this.data = model;
    }
}


