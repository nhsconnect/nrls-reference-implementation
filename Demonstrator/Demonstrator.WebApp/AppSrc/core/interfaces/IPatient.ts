import { IIdentifier } from "./fhir/IIdentifier";
import { IResourceReference } from "./fhir/IResourceReference";
import { IAddress } from "./fhir/IAddress";
import { IContactPoint } from "./fhir/IContactPoint";
import { IName } from "./fhir/IName";

export interface IPatient {
    resourceType: string;
    id: string;
    identifier: IIdentifier[];
    active: boolean;
    name: IName[];
    telecom: IContactPoint[];
    gender: string;
    birthDate: Date;
    deceasedBoolean: boolean;
    address: IAddress[];
    managingOrganization: IResourceReference;
    nhsNumber: string;
}