import { BaseGenericSystem } from "../BaseGenericSystem";

export class ASCallHandler extends BaseGenericSystem {
    data: any = {};

    activate(model) {
        this.data = model;
    }
}


