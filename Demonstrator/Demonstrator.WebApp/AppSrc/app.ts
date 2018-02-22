import { Aurelia, inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';
import { DialogRequested } from './core/helpers/EventMessages';

@inject(EventAggregator)
export class App {
    router: Router;

    constructor(private ea: EventAggregator) {
        ea.subscribe(DialogRequested, msg => this.showDialog(msg));
    }

    showDialog(msg) {
        console.log("Dialog Requested: ", msg);
    }

  configureRouter(config: RouterConfiguration, router: Router) {

      config.title = 'Demonstrator';

        var notFoundRoute = { route: 'error/404', moduleId: './error/index', title: 'Not Found', settings: { message: "Sorry, resource not found.", auth: false } };

        config.mapUnknownRoutes(notFoundRoute);

        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: './welcome/index', nav: true, title: 'Welcome' },
            { route: 'about', name: 'about', moduleId: './about/index', nav: true, title: 'About' },
            { route: 'developers', name: 'developers', moduleId: './developers/index', nav: true, title: 'Developers Area' },
            { route: 'actor-organisation', name: 'actor-organisation', moduleId: './actor-organisation/index', nav: false, title: 'Actor Organisation' },
            { route: 'actor-organisation/:actorOrgId', name: 'actor-organisation-personnel', moduleId: './actor-organisation/personnel', nav: false, title: 'Actor Organisation Personnel' },
            { route: 'personnel/:personnelId', name: 'personnel', moduleId: './personnel/index', nav: false, title: 'Personnel' },

            notFoundRoute
        ]);

        this.router = router;
  }
}
