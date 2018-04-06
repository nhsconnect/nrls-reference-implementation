export interface IRequest {
    headers: { [key: string]: string };
    resource?: any;
    id?: string;
    active?: boolean;
}