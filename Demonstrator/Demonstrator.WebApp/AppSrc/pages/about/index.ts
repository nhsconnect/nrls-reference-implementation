import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";
import { ConfigSvc } from "../../core/services/ConfigService";
import { inject } from "aurelia-framework";

@inject(ConfigSvc)
export class About {
    heading: string = 'About the NRLS';
    breadcrumb: Array<IBreadcrumb> = [];
    benefitfor: string = "ActorOrganisation";
    benefitforid: string = "5a8daa5c952e372cbdb317ea";
    dpiaLink: string;

    constructor(private configSvc: ConfigSvc) { }

    created() {
        this.setBreadcrumb();

        if (this.configSvc.config.DPIALink) {
            this.dpiaLink = this.configSvc.config.DPIALink;
        }
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'about', isActive: true });
    }

}


