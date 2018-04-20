import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";
import { ConfigSvc } from "../../core/services/ConfigService";
import { inject } from "aurelia-framework";

@inject(ConfigSvc)
export class AboutConsumersProviders {
    heading: string = 'Consumers & Providers';
    breadcrumb: Array<IBreadcrumb> = [];

    created() {
        this.setBreadcrumb();
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome' });
        this.breadcrumb.push(<IBreadcrumb>{ title: 'About', route: 'about', isActive: false, isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'about-consumers-providers', isActive: true });
    }

}


