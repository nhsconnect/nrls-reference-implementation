import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";
import { ConfigSvc } from "../../core/services/ConfigService";
import { inject } from "aurelia-framework";
import { AnalyticsSvc } from "../../core/services/AnalyticsService";

@inject(ConfigSvc, AnalyticsSvc)
export class AboutOnboarding {
    heading: string = 'Onboarding';
    breadcrumb: Array<IBreadcrumb> = [];
    dpiaLink: string;
    tomLink: string;

    constructor(private configSvc: ConfigSvc, private analyticsSvc: AnalyticsSvc) { }

    created() {
        this.setBreadcrumb();

        if (this.configSvc.config.DPIALink) {
            this.dpiaLink = this.configSvc.config.DPIALink;
        }

        if (this.configSvc.config.TOMLink) {
            this.tomLink = this.configSvc.config.TOMLink;
        }
    }

    private fileDownloadTrack(file: string) {
        this.analyticsSvc.downloads(file);

        return true;
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome' });
        this.breadcrumb.push(<IBreadcrumb>{ title: 'About', route: 'about', isActive: false, isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'about-onboarding', isActive: true });
    }

}


