import { BaseGenericSystem } from "../BaseGenericSystem";

export class GsMHTCareCoordinatorEPR extends BaseGenericSystem {
    data: any = {};

    activate(model) {
        this.data = model;
    }
}


