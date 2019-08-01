import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IActorOrganisation } from '../interfaces/IActorOrganisation';
import { IPersonnel } from '../interfaces/IPersonnel';

@inject(WebAPI)
export class ActorOrganisationSvc {

    baseUrl: string = 'ActorOrganisations';

    constructor(private api: WebAPI) { }

    getOne(actorOrgId: string) {
        let orgsPromise = this.api.do<IActorOrganisation>(`${this.baseUrl}/${actorOrgId}`, null, 'get');
        return orgsPromise;
    }

}