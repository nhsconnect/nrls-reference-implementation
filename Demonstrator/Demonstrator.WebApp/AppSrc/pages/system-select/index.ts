import { bindable, inject }     from "aurelia-framework";
import { IGenericSystem } from "../../core/interfaces/IGenericSystem";
import { GenericSystemSvc } from '../../core/services/GenericSystemService';
import { ISystemExplorer, ISystemExplorerItem } from "../../core/interfaces/ISystemExplorer";
import { ActorTypes } from "../../core/models/enums/ActorTypes";

@inject(GenericSystemSvc)
export class SystemSelect {
    heading: string = 'Demonstrator Systems';
    genericSystems: Array<IGenericSystem>;
    systemsLoading: boolean = false;
    systemCaseStudies: ISystemExplorer;
    systemTypes: any[] = [{ type: 'consumers', display: 'Consuming' }, { type: 'providers', display: 'Providing'}];

    constructor(private genericSystemSvc: GenericSystemSvc) {

        this.genericSystems = new Array<IGenericSystem>();
        this.systemCaseStudies = {
            consumers: new Array<ISystemExplorerItem>(),
            providers: new Array<ISystemExplorerItem>()
        };
    }

    created() {
        this.systemsLoading = true;

        this.getSystems().then(done => {

            this.buildSystemList();
        });
    }

    private getSystems(): Promise<boolean> {
        return this.genericSystemSvc.getAll().then(systems => {

            this.genericSystems = systems;

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
                moduleName: g.fModule
            };

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


