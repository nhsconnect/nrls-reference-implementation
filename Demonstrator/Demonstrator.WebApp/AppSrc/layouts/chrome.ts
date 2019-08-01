import { inject } from "aurelia-framework";
import { IDemonstratorConfig } from "../core/interfaces/IDemonstratorConfig";
import { EventAggregator, Subscription } from "aurelia-event-aggregator";
import { ConfigSvc } from "../core/services/ConfigService";
import { CheckAnnouncements } from "../core/helpers/EventMessages";
import { Router } from "aurelia-router";
import { AnalyticsSvc } from "../core/services/AnalyticsService";

@inject(EventAggregator, ConfigSvc, Router, AnalyticsSvc)
export class Chrome {

    canSeeAnnouncements: boolean = true;
    appConfig: IDemonstratorConfig;

    checkAnnouncementsSubscription: Subscription;

    constructor(private ea: EventAggregator, private configSvc: ConfigSvc, private router: Router, private analyticsSvc: AnalyticsSvc) {

        this.appConfig = configSvc.demoAppConfig;
    }

    attached() {
        this.checkAnnouncementsSubscription = this.ea.subscribe(CheckAnnouncements, () => {
            this.checkAnnouncements();
        });
    }

    detached() {
        this.checkAnnouncementsSubscription.dispose();
    }
 
    checkAnnouncements() {

        let canSeeAnnouncements = this.router.currentInstruction.config.settings.showAnnouncements;

        this.canSeeAnnouncements = (canSeeAnnouncements === undefined || canSeeAnnouncements === true);
    }
}


