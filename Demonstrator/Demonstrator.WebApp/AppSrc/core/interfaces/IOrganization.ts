import { IAddress } from "./fhir/IAddress";
import { IIdentifier } from "./fhir/IIdentifier";
import { IMeta } from "./fhir/IMeta";
import { IContactPoint } from "./fhir/IContactPoint";

export interface IOrganization {
    resourceType: string;
    id: string;
    meta: IMeta;
    extension: IExtension[];
    identifier: IIdentifier[];
    name: string;
    telecom: IContactPoint[];
    address: IAddress[];
}

export interface IExtension {
    url: string;
    valueString: string;
}