import { IContentView } from "./IContentView";

export interface IPersonnel {
    id: string;
    name: string;
    safeName: string;
    personnelType: string;
    imageUrl: string;
    context: Array<IContentView>;
    usesNrls: boolean;
    cModule: string;
    systemIds: Array<string>;
    actorOrganisationId: string;
}