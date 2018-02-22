export interface IPersonnel {
    id: string;
    name: string;
    imageUrl: string;
    context: string;
    usesNrls: boolean;
    systemIds: Array<string>;
    actorOrganisationId: string;
}