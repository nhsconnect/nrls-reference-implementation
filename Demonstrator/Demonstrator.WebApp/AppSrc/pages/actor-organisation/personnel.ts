import { ActorOrganisationSvc } from '../../core/services/ActorOrganisationService';
import { bindable, inject } from 'aurelia-framework';
import { IPersonnel } from '../../core/models/IPersonnel';

@inject(ActorOrganisationSvc)
export class ActorOrganisationPersonnel {
    heading: string = 'Actor Organisation Personnel';
    personnel: Array<IPersonnel>;
    actorOrgId: string;
    personnelListLoading: boolean = false;

    constructor(private actorOrgSvc: ActorOrganisationSvc) { }

    activate(params) {
        this.actorOrgId = params.actorOrgId;
    }

    created() {
        this.personnelListLoading = true;
        this.actorOrgSvc.getPersonnel(this.actorOrgId).then(personnel => {
            this.personnel = personnel;
            this.personnelListLoading = false;
        });
    }

}
