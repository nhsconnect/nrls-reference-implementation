import { IRecord } from "./IRecord";

export interface ICarePlan extends IRecord {
    involveFamilyOrCarer?: boolean;
    signsFeelingUnwell?: string;
    potentialTriggers?: string;
    whatHelpsInCrisis?: string;
    actionForDependants?: string,
    emergencyLocation?: string;
    emergencyNumber?: string; 
    crisisNumber?: string; 
    patientAcceptsPlan?: boolean;
    planCreatedBy?: string,
    planCreatedByJobTitle?: string,
    planCreated?: Date;
    planUpdated?: Date;
    active?: boolean
    patientNhsNumber?: string;

    crisisPlanRules: any;
}