import { Aurelia, inject, bindable, bindingMode } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';
import { DialogRequested } from './core/helpers/EventMessages';
import { IDialog } from './core/interfaces/IDialog';

@inject(EventAggregator)
export class App {
    router: Router;
    errorDialog: IDialog;

    constructor(private ea: EventAggregator) {
        ea.subscribe(DialogRequested, msg => {

            if (msg && msg.Severity != 'Information') {
                this.showErrorDialog(msg);
            }
        });
    }

    configureRouter(config: RouterConfiguration, router: Router) {

        config.title = 'NRLS Interactive Guide';

        var notFoundRoute = { route: 'error/404', moduleId: './pages/error/index', title: 'Not Found', settings: { message: "Sorry, resource not found.", auth: false } };

        config.mapUnknownRoutes(notFoundRoute);

        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: './pages/welcome/index', nav: true, title: 'Home' },
            { route: 'about', name: 'about', moduleId: './pages/about/index', nav: true, title: 'About' },
            { route: 'developers', name: 'developers', moduleId: './pages/developers/index', nav: true, title: 'Developers' },
            { route: 'actor-organisation/:routeParamId', name: 'actor-organisation-personnel', moduleId: './pages/actor-organisation/index', nav: false, title: 'Actor Organisation Personnel' },
            { route: 'personnel/:routeParamId', name: 'personnel', moduleId: './pages/personnel/index', nav: false, title: 'Personnel' },

            notFoundRoute
        ]);

        this.router = router;
    }

    showErrorDialog(msg) {
        this.errorDialog = <IDialog> {
            content: msg.dialog.Details,
            debug: msg.dialog.Diagnostics
        };
    }
}
