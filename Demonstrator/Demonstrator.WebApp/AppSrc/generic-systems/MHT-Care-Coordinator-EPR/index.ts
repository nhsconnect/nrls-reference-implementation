import { BaseGenericSystem } from "../BaseGenericSystem";
import { CrisisPlan } from "../../core/models/CrisisPlan";
import { ICarePlan } from "../../core/interfaces/ICarePlan";

export class GsMHTCareCoordinatorEPR extends BaseGenericSystem {
    data: any = {};
    crisisPlan: ICarePlan = new CrisisPlan();
    showCrisisPlan: boolean = true;
    showNrlsPointers: boolean = false;
    hasCrisisPlan: boolean = false;
    updatingPlan: boolean = false;
    allowCrisisPlan: boolean = false;

    activate(model) {
        this.data = model;
    }

    private patientChanged(newValue: string, oldValue: string): void {

        if (newValue === oldValue || !newValue) {
            return;
        }

        this.getCrisisPlan();
    }

    getCrisisPlan() {
        this.eprSvc.getPatientCrisisPlan(this.selectedPatient).then(crisisPlan => {
            if (typeof crisisPlan !== "string") {
                this.setCrisisPlan(crisisPlan);
                this.hasCrisisPlan = true;
            }
        });
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

        this.vdController.reset({ object: this.crisisPlan });
        this.vdController.validate({ object: this.crisisPlan, rules: this.crisisPlan.crisisPlanRules() })
            .then(result => {
                if (result.valid && this.patient) {

                    this.crisisPlan.patientNhsNumber = this.patient.identifier[0].value;
                    this.crisisPlan.planCreated = new Date();
                    this.crisisPlan.planUpdated = new Date();

                    this.crisisPlan.orgCode = this.data.organisation.orgCode;
                    this.crisisPlan.asid = this.data.genericSystem.asid;

                    this.crisisPlan.planCreatedBy = `${this.data.personnel.name.substr(0,1)} Jones`;
                    this.crisisPlan.planCreatedByJobTitle = this.data.personnel.name;

                    if (this.hasCrisisPlan) {
                        this.updatePlan();
                    } else {
                        this.createPlan();
                    }

                }
            });
    }

    deletePlan() {

        window.setTimeout(() => {

            this.eprSvc.delete(`${this.crisisPlan.id}`).then(planDelete => {
                this.updatingPlan = false;

                if (planDelete) {
                    this.crisisPlan = new CrisisPlan();
                    this.allowCrisisPlan = false;
                }
            });

        }, 250); //Looks better with delay :)

    }

    private createPlan() {
        this.updatingPlan = true;

        window.setTimeout(() => {

            this.eprSvc.create(this.crisisPlan).then(newPlan => {
                this.setCrisisPlan(newPlan);
                this.updatingPlan = false;
            });

        }, 250); //Looks better with delay :)
    }

    private updatePlan() {
        this.updatingPlan = true;

        window.setTimeout(() => {

            this.eprSvc.update(this.crisisPlan).then(newPlan => {
                this.setCrisisPlan(newPlan);
                this.updatingPlan = false;
            });

        }, 250); //Looks better with delay :)
    }

    private setCrisisPlan(crisisPlan: ICarePlan) {
        this.crisisPlan = new CrisisPlan(crisisPlan.id,
            this.data.organisation.orgCode,
            this.data.genericSystem.asid,
            crisisPlan.involveFamilyOrCarer,
            crisisPlan.signsFeelingUnwell,
            crisisPlan.potentialTriggers,
            crisisPlan.whatHelpsInCrisis,
            crisisPlan.actionForDependants,
            crisisPlan.emergencyLocation,
            crisisPlan.emergencyNumber,
            crisisPlan.crisisNumber,
            crisisPlan.patientAcceptsPlan,
            crisisPlan.planCreatedBy,
            crisisPlan.planCreatedByJobTitle,
            crisisPlan.planCreated,
            crisisPlan.planUpdated,
            crisisPlan.active,
            crisisPlan.patientNhsNumber);
    }
}


