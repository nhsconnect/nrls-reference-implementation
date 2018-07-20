import { Aurelia, inject, bindable, bindingMode } from 'aurelia-framework';
import { Router, RouterConfiguration, NavigationInstruction, Next, RouteConfig } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';
import { DialogRequested } from './core/helpers/EventMessages';
import { IDialog } from './core/interfaces/IDialog';
import { IDemonstratorConfig } from './core/interfaces/IDemonstratorConfig';

@inject(EventAggregator)
export class App {
    router: Router;
    errorDialog: IDialog;
    canShowContact: boolean = false;
    updatedAt?: string;
    appVersion?: string;
    baseUrl?: string;

    constructor(private ea: EventAggregator) {
        ea.subscribe(DialogRequested, msg => {

            if (msg && msg.Severity != 'Information') {
                this.showErrorDialog(msg);
            }
        });

        let demonstratorConfig = (window['demonstratorConfig'] || { updatedAt: '', appVersion: '', baseUrl: '' }) as IDemonstratorConfig;

        this.updatedAt = demonstratorConfig.updatedAt;
        this.appVersion = demonstratorConfig.appVersion;
        this.baseUrl = demonstratorConfig.baseUrl;
    }

    configureRouter(config: RouterConfiguration, router: Router) {

        config.title = 'NRLS Interactive Guide';
        config.options.hashChange = false;
        config.options.pushState = true;
        config.options.route = this.baseUrl;

        var notFoundRoute = { route: 'error/404', moduleId: './pages/error/index', title: 'Not Found', settings: { message: "Sorry, resource not found.", auth: false } };

        config.mapUnknownRoutes(<RouteConfig>notFoundRoute);

        config.addPipelineStep('postcomplete', PostCompleteStep);

        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: './pages/welcome/index', nav: true, title: 'Home' },
            { route: 'about', name: 'about', moduleId: './pages/about/index', nav: true, title: 'About' },
            { route: 'about/consumers-providers', name: 'about-consumers-providers', moduleId: './pages/about/consumers-providers', nav: false, title: 'About - Consumers Providers' },
            { route: 'about/onboarding', name: 'about-onboarding', moduleId: './pages/about/onboarding', nav: false, title: 'About - Onboarding' },
            { route: 'about/timeline', name: 'about-timeline', moduleId: './pages/about/timeline', nav: false, title: 'About - Timeline' },
            { route: 'about/benefits', name: 'about-benefits', moduleId: './pages/about/benefits', nav: false, title: 'About - Benefits' },

            { route: 'developers', name: 'developers', moduleId: './pages/developers/index', nav: true, title: 'Developers' },

            { route: 'actor-organisation/:routeParamId/:routeParamTitle?', name: 'actor-organisation-personnel', moduleId: './pages/actor-organisation/index', nav: false, title: 'Explore Organisation - Choose a Persona' },
            { route: 'personnel/:routeParamId/:routeParamTitle?', name: 'personnel', moduleId: './pages/personnel/index', nav: false, title: 'Explore Persona - What does the NRLS mean for me' },

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
    }
}

class PostCompleteStep {

    run(instruction: NavigationInstruction, next: Next) {

        if (!instruction.router.isNavigatingBack && !instruction.router.isNavigatingForward && !instruction.router.isNavigatingRefresh) {
            window.scrollTo(0, 0);
        }

        return next();
    }
}
