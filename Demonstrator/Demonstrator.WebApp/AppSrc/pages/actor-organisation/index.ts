import { ActorOrganisationSvc } from '../../core/services/ActorOrganisationService';
import { bindable, inject }     from 'aurelia-framework';
import { IPersonnel }           from '../../core/interfaces/IPersonnel';
import { IActorOrganisation }   from '../../core/interfaces/IActorOrganisation';

@inject(ActorOrganisationSvc)
export class ActorOrganisationPersonnel {
    heading: string = 'Organisation';
    organisation: IActorOrganisation;
    personnel: Array<IPersonnel>;
    actorOrgId: string;
    organisationLoading: boolean = false;
    personnelListLoading: boolean = false;

    constructor(private actorOrgSvc: ActorOrganisationSvc) { }

    activate(params) {
        this.actorOrgId = params.routeParamId;
    }

    created() {
        this.organisationLoading = true;
        this.actorOrgSvc.getOne(this.actorOrgId).then(organisation => {
            this.organisation = organisation;
            this.heading = this.organisation.name;
            this.organisationLoading = false;

            this.getPersonnel();
        });
    }

    private getPersonnel() {
        this.personnelListLoading = true;
        this.actorOrgSvc.getPersonnel(this.actorOrgId).then(personnel => {
            this.personnel = personnel;
            this.personnelListLoading = false;
        });
    }

}
