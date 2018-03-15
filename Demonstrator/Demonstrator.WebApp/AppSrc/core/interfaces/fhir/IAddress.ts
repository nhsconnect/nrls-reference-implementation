import { ICityElement } from "./ICityElement";
import { IPeriod } from "./IPeriod";

export interface IAddress {
    use: string;
    type: string;
    text?: string;
    line: ICityElement[];
    city: ICityElement;
    district: ICityElement;
    state: ICityElement;
    postalCode: ICityElement;
    country?: string;
    period: IPeriod;
    elementId?: string;
    id: string;
    extension: any[];
}