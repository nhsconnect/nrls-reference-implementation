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
    heading: string = 'System Demonstrator';
    personnel: IPersonnel;
    organisation: IActorOrganisation;
    genericSystem: IGenericSystem;
    genericSystemId: string;
    systemsLoading: boolean = false;
    systemModel: any = {};

    constructor(private personnelSvc: PersonnelSvc, private actorOrganisationSvc: ActorOrganisationSvc,
        private genericSystemSvc: GenericSystemSvc) { }

    activate(params) {
        this.genericSystemId = params.routeParamId;
        this.systemModel.standalone = true;
    }

    created() {
            
        this.systemsLoading = true;
        this.getSystem().then(done => {

            this.heading = `Demonstration system for a ${this.genericSystem.name}`;

            this.getPersonnel().then(done => {
                this.getOrganisation(this.personnel.actorOrganisationId).then(done => {
                    this.systemsLoading = false;
                });
            });
        });
   
    }

    private getOrganisation(orgId: string): Promise<boolean> {
        return this.actorOrganisationSvc.getOne(orgId).then(organisation => {
             this.systemModel.organisation = this.organisation = organisation;
            return true;
        });
    }

    private getPersonnel(): Promise<boolean> {
        return this.genericSystemSvc.getPersonnel(this.genericSystemId).then(personnel => {
            this.systemModel.personnel = this.personnel = personnel;
            return true;
        });
    }

    private getSystem(): Promise<boolean> {
        return this.genericSystemSvc.getOne(this.genericSystemId).then(system => {
            this.systemModel.genericSystem = this.genericSystem = system;
            return true;
        });
    }
}
