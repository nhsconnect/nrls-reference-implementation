import { ActorTypes } from "./enums/ActorTypes";

export interface IGenericSystem {
    id: string;
    name: string;
    imageUrl: string;
    context: string;
    fModule: string;
    Asid: string;
    actionTypeNames: Array<string>;
    actionTypes: Array<ActorTypes>;
}