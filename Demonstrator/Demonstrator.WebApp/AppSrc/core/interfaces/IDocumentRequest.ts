import { IRequest } from "./IRequest";

export interface IDocumentRequest extends IRequest {
    nhsNumber?: string;
}