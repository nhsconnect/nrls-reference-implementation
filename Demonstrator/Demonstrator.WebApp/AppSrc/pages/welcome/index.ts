import { IActorOrganisation }   from "../../core/interfaces/IActorOrganisation";
import { ActorOrganisationSvc } from "../../core/services/ActorOrganisationService";
import { bindable, inject }     from "aurelia-framework";

@inject(ActorOrganisationSvc)
export class Welcome {
    heading: string = 'Welcome to the NRLS Interactive Guide';
    actorOrganisations: Array<IActorOrganisation>;
    orgsLoading: boolean = false;

    constructor(private actorOrgSvc: ActorOrganisationSvc) { }

    created() {
        this.orgsLoading = true;
        this.actorOrgSvc.getAll().then(actorOrgs => {
            this.actorOrganisations = actorOrgs;
            this.orgsLoading = false;
        });
    }

}


