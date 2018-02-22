import { PersonnelSvc } from '../core/services/PersonnelService';
import { GenericSystemSvc } from '../core/services/GenericSystemService';
import { bindable, inject }     from 'aurelia-framework';
import { IPersonnel }           from '../core/models/IPersonnel';
import { IGenericSystem }       from '../core/models/IGenericSystem';
import { ActorTypes } from '../core/models/enums/ActorTypes';

@inject(PersonnelSvc, GenericSystemSvc)
export class Personnel {
    heading: string = 'Personnel';
    personnel: IPersonnel;
    providerSystems: Array<IGenericSystem> = [];
    consumerSystems: Array<IGenericSystem> = [];
    personnelId: string;
    systemsLoading: boolean = false;
    personnelLoading: boolean = false;

    constructor(private personnelSvc: PersonnelSvc, private genericSystemSvc: GenericSystemSvc) {    }

    activate(params) {
        this.personnelId = params.personnelId;
    }

    created() {
        this.personnelLoading = true;
        this.personnelSvc.get(this.personnelId).then(personnel => {

            this.personnel = personnel;
            this.personnelLoading = false;

            this.systemsLoading = true;
            this.genericSystemSvc.getList(this.personnel.systemIds).then(systems => {
                this.setSystems(systems);
                this.systemsLoading = false;
            });
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
