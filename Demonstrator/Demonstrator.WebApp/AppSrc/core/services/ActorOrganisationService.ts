import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IActorOrganisation } from '../models/IActorOrganisation';
import { IPersonnel } from '../models/IPersonnel';

@inject(WebAPI)
export class ActorOrganisationSvc {

    baseUrl: string = 'ActorOrganisations';

    constructor(private api: WebAPI) { }

    getAll() {
        let actorOrgs = this.api.do<Array<IActorOrganisation>>(`${this.baseUrl}`, null, 'get');
        return actorOrgs;
    }

    getPersonnel(actorOrgId: string) {
        let personnel = this.api.do<Array<IPersonnel>>(`${this.baseUrl}/${actorOrgId}/Personnel`, null, 'get');
        return personnel;
    }

}