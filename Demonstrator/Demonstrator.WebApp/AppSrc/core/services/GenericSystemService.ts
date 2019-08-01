import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IGenericSystem } from '../interfaces/IGenericSystem';
import { IPersonnel } from '../interfaces/IPersonnel';

@inject(WebAPI)
export class GenericSystemSvc {

    baseUrl: string = 'GenericSystems';

    constructor(private api: WebAPI) { }

    getAll() {
        let systems = this.api.do<Array<IGenericSystem>>(`${this.baseUrl}`, null, 'get');
        return systems;
    }

    getOne(systemId: string) {
        let system = this.api.do<IGenericSystem>(`${this.baseUrl}/${systemId}`, null, 'get');
        return system;
    }

    getPersonnel(systemId: string) {
        let personnel = this.api.do<IPersonnel>(`${this.baseUrl}/${systemId}/Personnel`, null, 'get');
        return personnel;
    }

}