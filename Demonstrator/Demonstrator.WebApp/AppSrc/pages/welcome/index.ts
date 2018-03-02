import { ScrollHints } from "../../core/helpers/ScrollHints";
import { IActorOrganisation } from "../../core/models/IActorOrganisation";
import { ActorOrganisationSvc } from "../../core/services/ActorOrganisationService";
import { inject } from "aurelia-framework";

@inject(ActorOrganisationSvc)
export class Welcome {
    heading: string = 'Welcome to the NRLS Interactive Guide';
    actorOrganisations: Array<IActorOrganisation>;
    orgsLoading: boolean = false;
    scrollHints: ScrollHints;

    constructor(private actorOrgSvc: ActorOrganisationSvc) {
        this.scrollHints = new ScrollHints();
    }

    created() {
        this.orgsLoading = true;
        this.actorOrgSvc.getAll().then(actorOrgs => {
            this.actorOrganisations = actorOrgs;
            this.orgsLoading = false;
        });
    }

    scrollHint() {
        this.scrollHints.scroll("#chooseYourOrganisation");
    }

    hideHint(e: JQueryEventObject) {

        this.scrollHints.hide(e.currentTarget, "#chooseYourOrganisation");
    }

}


