import { IActorOrganisation }   from "../../core/interfaces/IActorOrganisation";
import { ActorOrganisationSvc } from "../../core/services/ActorOrganisationService";
import { bindable, inject }     from "aurelia-framework";
import { IPersonnel } from "../../core/interfaces/IPersonnel";
import { IGenericSystem } from "../../core/interfaces/IGenericSystem";
import { GenericSystemSvc } from '../../core/services/GenericSystemService';
import { ISystemExplorer, ISystemExplorerItem, ISystemExplorerOrg } from "../../core/interfaces/ISystemExplorer";
import { ActorTypes } from "../../core/models/enums/ActorTypes";

@inject(ActorOrganisationSvc, GenericSystemSvc)
export class SystemSelect {
    heading: string = 'Demonstrator Systems';
    actorOrganisations: Array<IActorOrganisation>;
    personnel: Array<IPersonnel>;
    genericSystems: Array<IGenericSystem>;
    systemsLoading: boolean = false;
    systemCaseStudies: ISystemExplorer;
    systemTypes: any[] = [{ type: 'consumers', display: 'Consuming' }, { type: 'providers', display: 'Providing'}];
    orgPersonnelPasses: number = 0;

    constructor(private actorOrgSvc: ActorOrganisationSvc, private genericSystemSvc: GenericSystemSvc) {

        this.personnel = new Array<IPersonnel>();
        this.genericSystems = new Array<IGenericSystem>();
        this.systemCaseStudies = {
            consumers: new Array<ISystemExplorerItem>(),
            providers: new Array<ISystemExplorerItem>()
        };
    }

    created() {
        this.systemsLoading = true;
        this.getActorOrgs().then(done => {

            let orgTotal: number = this.actorOrganisations.length;
            let orgCount: number = 0;

            this.actorOrganisations.forEach(a => {
                this.getPersonnel(a.id).then(done => {

                    orgCount++;

                    if (orgCount === orgTotal) {
                        let personnelTotal: number = this.personnel.length;
                        let personnelCount: number = 0;

                        this.personnel.forEach(p => {
                            this.getSystems(p).then(done => {
                                personnelCount++;

                                if (personnelCount === personnelTotal) {
                                    this.buildSystemList();
                                }
                            });
                        });
                    }
                });
            });
        });
    }

    private getActorOrgs(): Promise<boolean> {
        return this.actorOrgSvc.getAll().then(actorOrgs => {
            this.actorOrganisations = actorOrgs;
            return true;
        });
    }

    private getPersonnel(actorOrgId: string): Promise<boolean> {
        return this.actorOrgSvc.getPersonnel(actorOrgId).then(personnel => {
            this.personnel = this.personnel.concat(personnel);
            return true;
        });
    }

    private getSystems(personnel: IPersonnel): Promise<boolean> {
        return this.genericSystemSvc.getList(personnel.systemIds).then(systems => {

            //clean up duplicates
            for (let key in systems) {
                if (systems.hasOwnProperty(key) && !this.genericSystems.find(g => g.id == systems[key].id)) {
                    this.genericSystems.push(systems[key]);
                }
            }
            return true;
        });
    }

    private buildSystemList():void {

        this.genericSystems.forEach(g => {

            //Create new system
            let system: ISystemExplorerItem = {
                name: g.name,
                id: g.id,
                safeName: g.safeName,
                healthContexts: new Array<ISystemExplorerOrg>()
            };

            //Map the personnel to an org
            let personOrgs: { [key: string]: IPersonnel[] } = {};

            this.personnel.forEach(p => {
                if (p.systemIds.includes(g.id)) {
                    if (!personOrgs[p.actorOrganisationId]) {
                        personOrgs[p.actorOrganisationId] = new Array<IPersonnel>();
                    }

                    personOrgs[p.actorOrganisationId].push(p);
                }
            });

            //Build the ISystemExplorerOrg
            for (let key in personOrgs) {

                if (personOrgs.hasOwnProperty(key)) {
                    let org: ISystemExplorerOrg = {
                        organisation: this.actorOrganisations.find(o => o.id == key) || <IActorOrganisation>{ id: key, name: "unknown" },
                        personnel: personOrgs[key]
                    };

                    system.healthContexts.push(org);
                }
            }

            //Add system to each actor types if they are of that type
            if (g.actionTypeNames.find(a => a === ActorTypes[ActorTypes.Consumer])) {
                this.systemCaseStudies.consumers.push(system);
            }

            if (g.actionTypeNames.find(a => a === ActorTypes[ActorTypes.Provider])) {
                this.systemCaseStudies.providers.push(system);
            }

        });

        this.systemsLoading = false;
        
    }

}


