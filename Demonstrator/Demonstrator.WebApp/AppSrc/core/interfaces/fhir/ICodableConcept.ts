import { ICoding } from "./ICoding";

export interface ICodableConcept {
    coding: ICoding[];
    text: string;
}