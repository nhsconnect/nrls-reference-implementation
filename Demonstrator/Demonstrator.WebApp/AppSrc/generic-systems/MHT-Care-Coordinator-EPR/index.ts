import { BaseGenericSystem } from "../BaseGenericSystem";
import { CrisisPlan } from "../../core/models/CrisisPlan";
import { ICarePlan } from "../../core/interfaces/ICarePlan";
import { IRequest } from "../../core/interfaces/IRequest";

export class GsMHTCareCoordinatorEPR extends BaseGenericSystem {
    data: any = {};
    crisisPlan: ICarePlan = new CrisisPlan();
    hasCrisisPlan: boolean = false;
    updatingPlan: boolean = false;
    allowCrisisPlan: boolean = false;
    showActionModal: boolean = false;

    activate(model) {
        this.data = model;
    }

    private patientChanged(newValue: string, oldValue: string): void {

        if (newValue === oldValue || !newValue || !this.selectedPatient) {
            return;
        }

        this.getCrisisPlan(`${this.selectedPatient.nhsNumber}`);
    }

    getCrisisPlan(nhsNumber: string) {
        this.eprSvc.getPatientCrisisPlan(nhsNumber).then(crisisPlan => {
            if (typeof crisisPlan !== "string") {
                this.setCrisisPlan(crisisPlan);
                this.hasCrisisPlan = true;
            }
        });
    }

    startView() {
        this.toggleInstructions();

        console.log(this.data);
        this.trackView("GsMHTCareCoordinatorEPR", "");
    }


    savePlan() {

        //this.vdController.reset({ object: this.crisisPlan });
        //this.vdController.validate({ object: this.crisisPlan, rules: this.crisisPlan.crisisPlanRules() })
        //    .then(result => {
        //        if (result.valid && this.patient) {

        //            this.crisisPlan.patientNhsNumber = this.patient.identifier[0].value;
        //            this.crisisPlan.planCreated = new Date();
        //            this.crisisPlan.planUpdated = new Date();

        //            this.crisisPlan.planCreatedBy = `${this.data.personnel.name.substr(0,1)} Jones`;
        //            this.crisisPlan.planCreatedByJobTitle = this.data.personnel.name;

        //            this.crisisPlan.cleaned();

        //            if (this.hasCrisisPlan) {
        //                this.updatePlan();
        //            } else {
        //                this.createPlan();
        //            }

        //        }
        //    });
    }

    deletePlan() {

        window.setTimeout(() => {

            this.eprSvc.delete(this.createRequest(null, this.crisisPlan.id)).then(planDelete => {
                this.updatingPlan = false;

                if (planDelete) {
                    this.crisisPlan = new CrisisPlan();
                    this.crisisPlan.version = "1";

                    this.allowCrisisPlan = false;
                    this.hasCrisisPlan = false;

                    this.setSystemMessage("Patient plan has been delete successfully.");
                }
            });

        }, 250); //Looks better with delay :)

    }

    private createPlan() {
        this.updatingPlan = true;

        window.setTimeout(() => {

            this.eprSvc.create(this.createRequest(this.crisisPlan, this.crisisPlan.id)).then(newPlan => {
                this.setCrisisPlan(newPlan);
                this.updatingPlan = false;
                this.hasCrisisPlan = true;

                this.setSystemMessage("Patient plan has been created successfully.");

                this.setActionModal();
            });

        }, 250); //Looks better with delay :)
    }

    private updatePlan() {
        this.updatingPlan = true;

        window.setTimeout(() => {

            this.eprSvc.update(this.createRequest(this.crisisPlan, this.crisisPlan.id)).then(newPlan => {
                this.setCrisisPlan(newPlan);
                this.updatingPlan = false;

                this.setSystemMessage("Patient plan has been updated successfully.");

                this.setActionModal();
            });

        }, 250); //Looks better with delay :)
    }

    private setActionModal() {
        window.setTimeout(() => {

            this.showActionModal = true;

        }, 250);
    }

    private createRequest(resource?: any, id?: string): IRequest {

        let request = <IRequest> {
            headers: {
                "Asid": this.data.genericSystem.asid,
                "OrgCode": this.data.organisation.orgCode
            },
            resource: resource,
            id: id
        };

        return request;
    }

    private setCrisisPlan(crisisPlan: ICarePlan) {
        this.crisisPlan = new CrisisPlan(crisisPlan.id,
            crisisPlan.version,
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

    deactivate() {
        this.data = undefined;
    }
}


