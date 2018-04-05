import { IContentView } from "./IContentView";

export interface IActorOrganisation {
    id: string;
    type: number;
    typeName: string;
    actorType: string;
    name: string;
    imageUrl: string;
    context: Array<IContentView>;
    orgCode: string;
}