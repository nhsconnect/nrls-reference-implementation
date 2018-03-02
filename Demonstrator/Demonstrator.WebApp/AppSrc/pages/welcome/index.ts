import { ScrollHints } from "../../core/helpers/ScrollHints";
import { IActorOrganisation } from "../../core/models/IActorOrganisation";
import { ActorOrganisationSvc } from "../../core/services/ActorOrganisationService";
import { bindable, inject } from "aurelia-framework";
import { PLATFORM } from 'aurelia-pal';

@inject(ActorOrganisationSvc)
export class Welcome {
    heading: string = 'Welcome to the NRLS Interactive Guide';
    actorOrganisations: Array<IActorOrganisation>;
    orgsLoading: boolean = false;
    scrollHints: any;
    hideHint: any;

    constructor(private actorOrgSvc: ActorOrganisationSvc) {
        
        this.hideHint = () => this._hideHint();
    }

    attached() {
        this.scrollHints = new ScrollHints();
        PLATFORM.addEventListener('scroll', this.hideHint);
    }

    detached() {
        PLATFORM.removeEventListener('scroll', this.hideHint);
    }

    created() {
        this.orgsLoading = true;
        this.actorOrgSvc.getAll().then(actorOrgs => {
            this.actorOrganisations = actorOrgs;
            this.orgsLoading = false;
        });
    }

    showHint() {
        this.scrollHints.scroll("#chooseYourOrganisation");
    }

    _hideHint() {

        this.scrollHints.hide("#chooseYourOrganisation");
    }

}


