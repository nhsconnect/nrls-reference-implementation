import { IBase } from "./IBase";

export interface IRecord extends IBase {
    id?: string;
    recordType?: string;
    version?: string;
}