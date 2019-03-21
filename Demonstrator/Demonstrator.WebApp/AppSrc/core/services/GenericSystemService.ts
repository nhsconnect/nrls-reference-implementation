import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IGenericSystem } from '../interfaces/IGenericSystem';
import { IPersonnel } from '../interfaces/IPersonnel';

@inject(WebAPI)
export class GenericSystemSvc {

    baseUrl: string = 'GenericSystems';

    constructor(private api: WebAPI) { }

    getOne(systemId: string) {
        let system = this.api.do<IGenericSystem>(`${this.baseUrl}/${systemId}`, null, 'get');
        return system;
    }

    getPersonnel(systemId: string) {
        let personnel = this.api.do<IPersonnel>(`${this.baseUrl}/${systemId}/Personnel`, null, 'get');
        return personnel;
    }

    getList(systemIds: Array<string>) {
        let systems = this.api.do<Array<IGenericSystem>>(`${this.baseUrl}`, { objectIds: systemIds }, 'post');
        return systems;
    }

}