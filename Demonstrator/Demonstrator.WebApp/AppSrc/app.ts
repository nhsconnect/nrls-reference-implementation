import { Aurelia, inject, bindable, bindingMode } from 'aurelia-framework';
import { Router, RouterConfiguration, NavigationInstruction, Next, RouteConfig } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { DialogRequested, CookieCanTrack, CheckAnnouncements, SystemError } from './core/helpers/EventMessages';
import { IDialog } from './core/interfaces/IDialog';
import { IDemonstratorConfig } from './core/interfaces/IDemonstratorConfig';
import { AnalyticsSvc } from './core/services/AnalyticsService';
import { CookieSvc } from './core/services/CookieService';
import { ConfigSvc } from "./core/services/ConfigService";


@inject(EventAggregator, AnalyticsSvc, CookieSvc, ConfigSvc)
export class App {

    hideBanner: boolean;
    router: Router;
    errorDialog: IDialog;
    handleAcceptTrack: any;
    appConfig: IDemonstratorConfig;

    dialogRequestedSubscription: Subscription;
    cookieCanTrackSubscription: Subscription;

    constructor(private ea: EventAggregator, private analyticsSvc: AnalyticsSvc, private cookieSvc: CookieSvc, private configSvc: ConfigSvc) {

        this.appConfig = configSvc.demoAppConfig;

        this.handleAcceptTrack = e => {
            this.ea.publish(new CookieCanTrack(this.cookieSvc.canTrack));
        };

    }

    attached() {

        this.cookieSvc.start(this.appConfig.cookieBotId);

        window.addEventListener('CookiebotOnAccept', this.handleAcceptTrack);
        window.addEventListener('CookiebotOnDecline', this.handleAcceptTrack); 

        window['CookiebotCallback_OnDecline'] = () => {
            this.handleAcceptTrack();
        }

        this.dialogRequestedSubscription = this.ea.subscribe(DialogRequested, msg => {

            if (msg && msg.severity != 'Information') {
                this.showErrorDialog(msg);
            }
        });

        this.cookieCanTrackSubscription = this.ea.subscribe(CookieCanTrack, cct => {
            if (cct.allowed) {
                this.analyticsSvc.start(cct.allowed, () => {
                    this.cookieSvc.runScripts();
                    this.setPageTrack();
                });
            } else {
                this.analyticsSvc.stop(() => {
                    this.cookieSvc.runScripts();
                });
            }
        });
    }

    detached() {
        window.removeEventListener('CookiebotOnAccept', this.handleAcceptTrack);
        window.removeEventListener('CookiebotOnDecline', this.handleAcceptTrack);

        window['CookiebotCallback_OnDecline'] = undefined;

        this.dialogRequestedSubscription.dispose();
        this.cookieCanTrackSubscription.dispose();
    }

    setPageTrack() {

        let pageTitleSlice = this.router.currentInstruction.config.settings.dynamicTitle ? this.router.currentInstruction.params.routeParamTitle : undefined;

        this.analyticsSvc.trackPage(pageTitleSlice);
    }

    configureRouter(config: RouterConfiguration, router: Router) {

        config.title = 'NRL Interactive Guide';
        config.options.hashChange = false;
        config.options.pushState = true;
        config.options.route = this.appConfig.baseUrl;

        var notFoundRoute = { route: 'error/404', moduleId: './pages/error/index', title: 'Not Found', settings: { message: "Sorry, resource not found.", auth: false } };

        config.mapUnknownRoutes(<RouteConfig>notFoundRoute);

        config.addPipelineStep('preActivate', StartAnalyticsStep);
        config.addPipelineStep('postRender', EndAnalyticsStep);
        config.addPipelineStep('postRender', CheckAnnouncementsStep);
        config.addPipelineStep('postRender', ScrollPageStep);

        config.map([
          { route: "", redirect: "system-select" },
          {
            route: "system-select",
            name: "system-select",
            moduleId: "./pages/system-select/index",
            nav: false,
            title: "Demonstrator Explorer",
          },
          {
            route: "system-demo/:routeParamId/:routeParamTitle?",
            name: "system-demo",
            moduleId: "./pages/system-demo/index",
            nav: false,
            title: "Demonstrator System Example",
            settings: { dynamicTitle: true },
          },

          notFoundRoute,
        ]);

        this.router = router;
    }

    showErrorDialog(msg) {
        this.errorDialog = <IDialog> {
            details: msg.dialog.details,
            debug: msg.dialog.diagnostics
        };

        this.ea.publish(new SystemError(null));
    }

    closeBanner() {
        this.hideBanner = true;
    }
}

@inject(AnalyticsSvc)
class StartAnalyticsStep {

    constructor(private gaSvc: AnalyticsSvc) {}

    run(instruction: NavigationInstruction, next: Next) {

        this.gaSvc.startLoadTime();

        return next();
    }
}

@inject(AnalyticsSvc)
class EndAnalyticsStep {

    constructor(private gaSvc: AnalyticsSvc) {}

    run(inst: NavigationInstruction, next: Next) {

        this.gaSvc.stopLoadTime();

        let pageTitleSlice = inst.router.currentInstruction.config.settings.dynamicTitle ? inst.router.currentInstruction.params.routeParamTitle : undefined;

        this.gaSvc.trackPage(pageTitleSlice);

        this.gaSvc.clearInitTime();

        return next();
    }

}

@inject(EventAggregator)
class CheckAnnouncementsStep {

    constructor(private ea: EventAggregator) { }

    run(inst: NavigationInstruction, next: Next) {

        this.ea.publish(new CheckAnnouncements());

        return next();
    }

}

class ScrollPageStep {

    run(instruction: NavigationInstruction, next: Next) {

        if (!instruction.router.isNavigatingBack && !instruction.router.isNavigatingForward && !instruction.router.isNavigatingRefresh) {
            window.scrollTo(0, 0);
        }

        return next();
    }
}
