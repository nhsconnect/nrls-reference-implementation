import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IPersonnel } from '../interfaces/IPersonnel';

@inject(WebAPI)
export class PersonnelSvc {

    baseUrl: string = 'Personnel';

    constructor(private api: WebAPI) { }

    get(personnelId: string) {
        let personnel = this.api.do<IPersonnel>(`${this.baseUrl}/${personnelId}`, null, 'get');
        return personnel;
    }

}