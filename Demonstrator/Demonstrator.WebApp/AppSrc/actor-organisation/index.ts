import { ActorOrganisationSvc } from '../core/services/ActorOrganisationService';
import { bindable, inject } from 'aurelia-framework';
import { IActorOrganisation } from '../core/models/IActorOrganisation';

@inject(ActorOrganisationSvc)
export class ActorOrganisation {
    heading: string = 'Actor Organisation';
    actorOrganisations: Array<IActorOrganisation>;
    orgsLoading: boolean = false;

    constructor(private actorOrgSvc: ActorOrganisationSvc) { }

    activate() {
    }

    created() {
        this.orgsLoading = true;
        this.actorOrgSvc.getAll().then(actorOrgs => {
            this.actorOrganisations = actorOrgs;
            this.orgsLoading = false;
        });
    }

}
