import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IGenericSystem } from '../models/IGenericSystem';

@inject(WebAPI)
export class GenericSystemSvc {

    baseUrl: string = 'GenericSystems';

    constructor(private api: WebAPI) { }

    getList(systemIds: Array<string>) {
        let systems = this.api.do<Array<IGenericSystem>>(`${this.baseUrl}`, { objectIds: systemIds }, 'post');
        return systems;
    }

}