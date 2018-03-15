import { IPeriod } from "./IPeriod";

export interface IName {
    use: string;
    text: null;
    family?: string;
    given: string[];
    prefix: any[];
    suffix: any[];
    period?: IPeriod;
    id: null;
    extension: any[];
}