import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";
import { ConfigSvc } from "../../core/services/ConfigService";
import { inject } from "aurelia-framework";

@inject(ConfigSvc)
export class AboutOnboarding {
    heading: string = 'Onboarding';
    breadcrumb: Array<IBreadcrumb> = [];
    dpiaLink: string;

    constructor(private configSvc: ConfigSvc) { }

    created() {
        this.setBreadcrumb();

        if (this.configSvc.config.DPIALink) {
            this.dpiaLink = this.configSvc.config.DPIALink;
        }
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome' });
        this.breadcrumb.push(<IBreadcrumb>{ title: 'About', route: 'about', isActive: false, isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'about-onboarding', isActive: true });
    }

}


