import { PersonnelSvc }         from '../../core/services/PersonnelService';
import { ActorOrganisationSvc } from '../../core/services/ActorOrganisationService';
import { GenericSystemSvc }     from '../../core/services/GenericSystemService';
import { bindable, inject }     from 'aurelia-framework';
import { IPersonnel }           from '../../core/interfaces/IPersonnel';
import { IGenericSystem }       from '../../core/interfaces/IGenericSystem';
import { ActorTypes }           from '../../core/models/enums/ActorTypes';
import { IActorOrganisation }   from '../../core/interfaces/IActorOrganisation';

@inject(PersonnelSvc, ActorOrganisationSvc, GenericSystemSvc)
export class Personnel {
    heading: string = 'Personnel';
    personnel: IPersonnel;
    organisation: IActorOrganisation;
    providerSystems: Array<IGenericSystem> = [];
    consumerSystems: Array<IGenericSystem> = [];
    personnelId: string;
    systemsLoading: boolean = false;
    personnelLoading: boolean = false;

    constructor(private personnelSvc: PersonnelSvc, private actorOrganisationSvc: ActorOrganisationSvc, private genericSystemSvc: GenericSystemSvc) {}

    activate(params) {
        this.personnelId = params.routeParamId;
    }

    created() {
        this.personnelLoading = true;
        this.personnelSvc.get(this.personnelId).then(personnel => {

            this.personnel = personnel;
            this.heading = this.personnel.name;
            this.personnelLoading = false;

            this.getOrganisation(this.personnel.actorOrganisationId);
            this.getSystems();
        });      
    }

    private getOrganisation(orgId: string) {
        this.actorOrganisationSvc.getOne(orgId).then(organisation => this.organisation = organisation);
    }

    private getSystems() {
        this.systemsLoading = true;
        this.genericSystemSvc.getList(this.personnel.systemIds).then(systems => {
            this.setSystems(systems);
            this.systemsLoading = false;
        });
    }

    private setSystems(systems: Array<IGenericSystem>) : void {

        if (systems && systems.length > 0) {

            systems.forEach(s => {

                if (s.actionTypes.indexOf(ActorTypes.Consumer) > -1) {
                    this.consumerSystems.push(s);
                }

                if (s.actionTypes.indexOf(ActorTypes.Provider) > -1) {
                    this.providerSystems.push(s);
                }
            });
        }
        
    }

}
