import { ActorTypes } from "../models/enums/ActorTypes";

export interface IGenericSystem {
    id: string;
    name: string;
    safeName: string;
    imageUrl: string;
    context: string;
    fModule: string;
    Asid: string;
    actionTypeNames: Array<string>;
    actionTypes: Array<ActorTypes>;
}