import { IPeriod } from "./IPeriod";

export interface IContactPoint {
    resourceType: string;
    system: string;
    value: string;
    use: string;
    rank: string;
    period: IPeriod;
    elementId: null;
    extension: any[];
    id: any;
}