export interface ICarePlan {
    id?: string;
    involveFamilyOrCarer?: boolean;
    signsFeelingUnwell?: string;
    potentialTriggers?: string;
    whatHelpsInCrisis?: string;
    emergencyLocation?: string;
    emergencyNumber?: string; 
    crisisNumber?: string; 
    patientAcceptsPlan?: boolean;
    planCreated?: Date;
    planUpdated?: Date;
    active?: boolean
    patientNhsNumber?: string;
}