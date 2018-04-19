import { IRecord } from "../interfaces/IRecord";
import { BaseModel } from "./Base";

export class MedicalRecord extends BaseModel {
    constructor(public id?: string,
        public recordType?: string,
        public version?: string) {

        super();
    }
}