import { IActorOrganisation } from "./IActorOrganisation";
import { IPersonnel } from "./IPersonnel";

export interface ISystemExplorer {
    consumers: ISystemExplorerItem[];
    providers: ISystemExplorerItem[];
}

export interface ISystemExplorerItem {
    id: string;
    name: string;
    safeName: string;
    moduleName: string;
    //healthContexts: ISystemExplorerOrg[];
}

//export interface ISystemExplorerOrg {
//    organisation: IActorOrganisation;
//    personnel: IPersonnel[];
//}