import { IContentView } from "./IContentView";

export interface IActorOrganisation {
    id: string;
    type: number;
    typeName: string;
    actorType: string;
    name: string;
    safeName: string;
    imageUrl: string;
    context: Array<IContentView>;
    orgCode: string;
    personnelLinkId: string;
}