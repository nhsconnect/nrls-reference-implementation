import { BaseGenericSystem } from "../BaseGenericSystem";
import { CarePlan } from "../../core/models/CarePlan";


export class GsMHTCareCoordinatorEPR extends BaseGenericSystem {
    data: any = {};
    carePlan: CarePlan = new CarePlan();
    showCrisisPlan: boolean = true;
    showNrlsPointers: boolean = false;

    activate(model) {
        this.data = model;
    }

    showPointersPanel() {
        this.showCrisisPlan = false;
        this.showNrlsPointers = true;
    }

    showCrisisPanel() {
        this.showCrisisPlan = true;
        this.showNrlsPointers = false;
    }

    savePlan() {

        this.vdController.reset({ object: this.carePlan });
        this.vdController.validate({ object: this.carePlan, rules: this.carePlan.carePlanRules() })
            .then(result => {
                if (result.valid && this.patient) {

                    this.carePlan.patientNhsNumber = this.patient.identifier[0].value;
                    this.carePlan.planCreated = new Date();
                    this.carePlan.planUpdated = new Date();

                    console.log(this.carePlan);
                } else {
                    console.log("invalid");
                }
            });
    }
}


