import { IRecord } from "../interfaces/IRecord";

export class MedicalRecord {
    constructor(public id?: string,
        public recordType?: string,
        public orgCode?: string,
        public asid?: string) {
    }
}