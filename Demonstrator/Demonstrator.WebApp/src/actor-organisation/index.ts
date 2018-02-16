import { ActorOrganisationSvc } from '../core/services/ActorOrganisationService';
import { bindable, inject } from 'aurelia-framework';
import { IActorOrganisation } from '../core/models/IActorOrganisation';

@inject(ActorOrganisationSvc)
export class ActorOrganisation {
    heading: string = 'Actor Organisation';
    actorOrganisations: Array<IActorOrganisation>;
    actorType: string;

    constructor(private actorOrgSvc: ActorOrganisationSvc) { }

    activate(params) {
        this.actorType = params.actorType;
    }

    created() {
        this.actorOrgSvc.getAll(this.actorType).then(actorOrgs => this.actorOrganisations = actorOrgs);
    }

}
