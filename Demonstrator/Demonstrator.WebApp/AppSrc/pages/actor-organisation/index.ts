import { ActorOrganisationSvc } from '../../core/services/ActorOrganisationService';
import { bindable, inject } from 'aurelia-framework';
import { IPersonnel }           from '../../core/interfaces/IPersonnel';
import { IActorOrganisation }   from '../../core/interfaces/IActorOrganisation';
import { IBreadcrumb } from '../../core/interfaces/IBreadcrumb';
import { AnalyticsSvc } from '../../core/services/AnalyticsService';

@inject(ActorOrganisationSvc, AnalyticsSvc)
export class ActorOrganisationPersonnel {
    heading: string = 'Organisation';
    organisation: IActorOrganisation;
    personnel: Array<IPersonnel>;
    actorOrgId: string;
    organisationLoading: boolean = false;
    personnelListLoading: boolean = false;
    breadcrumb: Array<IBreadcrumb> = [];
    benefitsForName: string;

    constructor(private actorOrgSvc: ActorOrganisationSvc, private analyticsSvc: AnalyticsSvc) {
        
    }

    activate(params) {
        this.actorOrgId = params.routeParamId;
    }

    created() {
        this.organisationLoading = true;
        this.actorOrgSvc.getOne(this.actorOrgId).then(organisation => {
            this.organisation = organisation;
            this.benefitsForName = `the ${this.organisation.name}`;
            this.heading = this.organisation.name;
            this.organisationLoading = false;

            this.setBreadcrumb();

            this.getPersonnel();
        });
    }

    private getPersonnel() : void {
        this.personnelListLoading = true;
        this.actorOrgSvc.getPersonnel(this.actorOrgId).then(personnel => {
            this.personnel = personnel;
            this.personnelListLoading = false;
        });
    }

    private setBreadcrumb() : void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Choose a Persona', isActive: true });
    }

    private recordBenefitView() {

        this.analyticsSvc.benefitsButton(this.organisation.name);
    }

}
