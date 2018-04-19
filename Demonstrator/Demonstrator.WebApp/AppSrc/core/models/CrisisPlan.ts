import { ValidationRules } from "aurelia-validation";
import { ICarePlan } from "../interfaces/ICarePlan";
import { MedicalRecord } from "./MedicalRecord";

export class CrisisPlan extends MedicalRecord implements ICarePlan {

    constructor(public id?: string,
                public version?: string,
                public involveFamilyOrCarer?: boolean,
                public signsFeelingUnwell?: string,
                public potentialTriggers?: string,
                public whatHelpsInCrisis?: string,
                public actionForDependants?: string,
                public emergencyLocation?: string,
                public emergencyNumber?: string,
                public crisisNumber?: string,
                public patientAcceptsPlan?: boolean,
                public planCreatedBy?: string,
                public planCreatedByJobTitle?: string,
                public planCreated?: Date,
                public planUpdated?: Date,
                public active?: boolean,
                public patientNhsNumber?: string) {

        super(id, 'MentalHealthCrisisPlan', version);
    }

    public cleaned() {

        this.signsFeelingUnwell = this.cleanContent(this.signsFeelingUnwell);
        this.potentialTriggers = this.cleanContent(this.potentialTriggers);
        this.whatHelpsInCrisis = this.cleanContent(this.whatHelpsInCrisis);
        this.actionForDependants = this.cleanContent(this.actionForDependants);

        this.emergencyLocation = this.cleanContent(this.emergencyLocation);
        this.emergencyNumber = this.cleanContent(this.emergencyNumber);
        this.crisisNumber = this.cleanContent(this.crisisNumber);
    }

    public crisisPlanRules() {
        const crisisPlanRules = ValidationRules
            .ensure('involveFamilyOrCarer').required()
            .ensure('signsFeelingUnwell').required()
            .ensure('potentialTriggers').required()
            .ensure('whatHelpsInCrisis').required()
            .ensure('patientAcceptsPlan').required()
            .rules;

        return crisisPlanRules;
    }
}