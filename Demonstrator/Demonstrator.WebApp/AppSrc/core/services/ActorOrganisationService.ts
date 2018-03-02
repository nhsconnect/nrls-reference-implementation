import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IActorOrganisation } from '../models/IActorOrganisation';
import { IPersonnel } from '../models/IPersonnel';

@inject(WebAPI)
export class ActorOrganisationSvc {

    baseUrl: string = 'ActorOrganisations';

    constructor(private api: WebAPI) { }

    getAll() {
        let actorOrgsPromise = this.api.do<Array<IActorOrganisation>>(`${this.baseUrl}`, null, 'get');
        return actorOrgsPromise;
    }

    getOne(actorOrgId: string) {
        let orgsPromise = this.api.do<IActorOrganisation>(`${this.baseUrl}/${actorOrgId}`, null, 'get');
        return orgsPromise;
    }

    getPersonnel(actorOrgId: string) {
        let personnelPromise = this.api.do<Array<IPersonnel>>(`${this.baseUrl}/${actorOrgId}/Personnel`, null, 'get');
        return personnelPromise;
    }

}