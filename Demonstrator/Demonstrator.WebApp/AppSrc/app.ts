import { Aurelia, inject, bindable, bindingMode } from 'aurelia-framework';
import { Router, RouterConfiguration, NavigationInstruction, Next, RouteConfig } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';
import { DialogRequested, CookieCanTrack } from './core/helpers/EventMessages';
import { IDialog } from './core/interfaces/IDialog';
import { IDemonstratorConfig } from './core/interfaces/IDemonstratorConfig';
import { AnalyticsSvc } from './core/services/AnalyticsService';
import { DemonstratorConfig } from './core/models/DemonstratorConfig';
import { CookieSvc } from './core/services/CookieService';

@inject(EventAggregator, AnalyticsSvc, CookieSvc)
export class App {
    router: Router;
    errorDialog: IDialog;
    canShowContact: boolean = false;
    handleAcceptTrack: any;

    appConfig: IDemonstratorConfig;

    constructor(private ea: EventAggregator, private analyticsSvc: AnalyticsSvc, private cookieSvc: CookieSvc) {
        ea.subscribe(DialogRequested, msg => {

            if (msg && msg.Severity != 'Information') {
                this.showErrorDialog(msg);
            }
        });

        ea.subscribe(CookieCanTrack, cct => {

            if (cct.allowed) {
                this.analyticsSvc.start(cct.allowed, () =>
                {
                    this.cookieSvc.runScripts();
                    this.setPageTrack();
                });
            } else {
                this.analyticsSvc.stop(() => {
                    this.cookieSvc.runScripts();
                });
            }
        });

        this.handleAcceptTrack = e => {
            this.ea.publish(new CookieCanTrack(this.cookieSvc.canTrack));
        };

        this.appConfig = (window['demonstratorConfig'] || new DemonstratorConfig());
    }

    attached() {
        this.cookieSvc.start(this.appConfig.cookieBotId);

        window.addEventListener('CookiebotOnAccept', this.handleAcceptTrack);
        window.addEventListener('CookiebotOnDecline', this.handleAcceptTrack); 
    }

    detached() {
        window.removeEventListener('CookiebotOnAccept', this.handleAcceptTrack);
        window.removeEventListener('CookiebotOnDecline', this.handleAcceptTrack);
    }

    setPageTrack() {

        let pageTitleSlice = this.router.currentInstruction.config.settings.dynamicTitle ? this.router.currentInstruction.params.routeParamTitle : undefined;

        this.analyticsSvc.trackPage(pageTitleSlice);
    }

    configureRouter(config: RouterConfiguration, router: Router) {

        config.title = 'NRLS Interactive Guide';
        config.options.hashChange = false;
        config.options.pushState = true;
        config.options.route = this.appConfig.baseUrl;

        var notFoundRoute = { route: 'error/404', moduleId: './pages/error/index', title: 'Not Found', settings: { message: "Sorry, resource not found.", auth: false } };

        config.mapUnknownRoutes(<RouteConfig>notFoundRoute);

        config.addPipelineStep('preActivate', StartAnalyticsStep);
        config.addPipelineStep('postRender', EndAnalyticsStep);
        config.addPipelineStep('postRender', ScrollPageStep);

        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: './pages/welcome/index', nav: true, title: 'Home' },
            { route: 'about', name: 'about', moduleId: './pages/about/index', nav: true, title: 'About' },
            { route: 'about/consumers-providers', name: 'about-consumers-providers', moduleId: './pages/about/consumers-providers', nav: false, title: 'About - Consumers Providers' },
            { route: 'about/onboarding', name: 'about-onboarding', moduleId: './pages/about/onboarding', nav: false, title: 'About - Onboarding' },
            { route: 'about/timeline', name: 'about-timeline', moduleId: './pages/about/timeline', nav: false, title: 'About - Timeline' },
            { route: 'about/benefits', name: 'about-benefits', moduleId: './pages/about/benefits', nav: false, title: 'About - Benefits' },

            { route: 'developers', name: 'developers', moduleId: './pages/developers/index', nav: true, title: 'Developers' },

            { route: 'actor-organisation/:routeParamId/:routeParamTitle?', name: 'actor-organisation-personnel', moduleId: './pages/actor-organisation/index', nav: false, title: 'Explore Organisation - Choose a Persona', settings: { dynamicTitle: true } },
            { route: 'personnel/:routeParamId/:routeParamTitle?', name: 'personnel', moduleId: './pages/personnel/index', nav: false, title: 'Explore Persona - What does the NRLS mean for me', settings: { dynamicTitle: true } },

            { route: 'privacy-policy', name: 'privacy-policy', moduleId: './pages/privacy-policy/index', nav: false, title: 'Privacy Policy' },
            { route: 'cookie-policy', name: 'cookie-policy', moduleId: './pages/cookie-policy/index', nav: false, title: 'Cookie Policy' },
            { route: 'accessibility', name: 'accessibility', moduleId: './pages/accessibility/index', nav: false, title: 'Accessibility' },

            notFoundRoute
        ]);

        this.router = router;
    }

    showErrorDialog(msg) {
        this.errorDialog = <IDialog> {
            details: msg.dialog.Details,
            debug: msg.dialog.Diagnostics
        };
    }

    showContactDialog() {
        this.canShowContact = !this.canShowContact;

        if (this.canShowContact === true) {
            this.analyticsSvc.contactsModal(window.location.pathname);
        }
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

class ScrollPageStep {

    run(instruction: NavigationInstruction, next: Next) {

        if (!instruction.router.isNavigatingBack && !instruction.router.isNavigatingForward && !instruction.router.isNavigatingRefresh) {
            window.scrollTo(0, 0);
        }

        return next();
    }
}
