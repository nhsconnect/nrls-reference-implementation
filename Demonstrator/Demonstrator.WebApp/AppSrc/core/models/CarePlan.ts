import { ValidationRules } from "aurelia-validation";
import { ICarePlan } from "../interfaces/ICarePlan";

export class CarePlan implements ICarePlan {
    constructor(public id?: string,
                public involveFamilyOrCarer?: boolean,
                public signsFeelingUnwell?: string,
                public potentialTriggers?: string,
                public whatHelpsInCrisis?: string,
                public emergencyLocation?: string,
                public emergencyNumber?: string,
                public crisisNumber?: string,
                public patientAcceptsPlan?: boolean,
                public planCreated?: Date,
                public planUpdated?: Date,
                public active?: boolean,
                public patientNhsNumber?: string) {
    }

    public carePlanRules() {
        const carePlanRules = ValidationRules
            .ensure('involveFamilyOrCarer').required()
            .ensure('signsFeelingUnwell').required()
            .ensure('potentialTriggers').required()
            .ensure('whatHelpsInCrisis').required()
            .ensure('patientAcceptsPlan').required()
            .rules;

        return carePlanRules;
    }
}