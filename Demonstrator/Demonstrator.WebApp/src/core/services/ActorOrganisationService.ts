import { WebAPI } from '../WebApi';
import { bindable, inject } from 'aurelia-framework';
import { IActorOrganisation } from '../models/IActorOrganisation';

@inject(WebAPI)
export class ActorOrganisationSvc {

    baseUrl: string = 'ActorOrganisations';

    constructor(private api: WebAPI) { }

    getAll(actorType: string) {
        let actorOrgs = this.api.do<Array<IActorOrganisation>>(`${this.baseUrl}/${actorType}`, null, 'get');
        return actorOrgs;
    }

}