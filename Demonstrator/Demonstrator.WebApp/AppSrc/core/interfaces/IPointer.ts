import { IMeta } from "./fhir/IMeta";
import { IIdentifier } from "./fhir/IIdentifier";
import { ICodableConcept } from "./fhir/ICodableConcept";
import { IResourceReference } from "./fhir/IResourceReference";
import { IPatient } from "./IPatient";
import { IOrganization } from "./IOrganization";

export interface IPointer {
    resourceType: string;
    id: string;
    meta: IMeta;
    identifier: IIdentifier[];
    status: string;
    type: ICodableConcept;
    subject: IResourceReference;
    created: string; //date
    indexed: string; //date
    author: IResourceReference;
    custodian: IResourceReference;
    content: IContent[];

    subjectViewModel: IPatient;
    custodianViewModel: IOrganization;
    authorViewModel: IOrganization;

    practiceSetting: string;
    contactUrl: string;
}


export interface IContent {
    attachment: IAttachment;
}

export interface IAttachment {
    contentType: string;
    url: string;
    title: string;
    creation: string;
}

