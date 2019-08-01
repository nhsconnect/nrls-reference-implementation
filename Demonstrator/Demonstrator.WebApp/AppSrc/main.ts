import { Aurelia } from 'aurelia-framework';
import { ConfigSvc } from "./core/services/ConfigService";
import { Container } from 'aurelia-dependency-injection';

// we want font-awesome to load as soon as possible to show the fa-spinner
import 'font-awesome/css/font-awesome.css';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import '../AppStyles/app.scss';

// comment out if you don't want a Promise polyfill (remove also from webpack.config.js)
import * as Bluebird from 'bluebird';
import { IWebAppConfig } from './core/interfaces/IWebAppConfig';
import { IConfigSvc } from './core/interfaces/IConfigSvc';
Bluebird.config({ warnings: false });

export async function configure(aurelia: Aurelia) {

    let configSvc: IConfigSvc = Container.instance.get(ConfigSvc);
    
    let debugLevel = configSvc && configSvc.webAppConfig && configSvc.webAppConfig.ENV !== "production" ? undefined : 'none';

    aurelia.use
        .standardConfiguration()
        .developmentLogging(debugLevel)
        .plugin('core/helpers/converters/index')
        .plugin('core/helpers/loaders/index')
        .plugin('core/includes');
        //.plugin('aurelia-validation');

  // Uncomment the line below to enable animation.
  // aurelia.use.plugin('aurelia-animator-css');
  // if the css animator is enabled, add swap-order="after" to all router-view elements

  // Anyone wanting to use HTMLImports to load views, will need to install the following plugin.
  // aurelia.use.plugin('aurelia-html-import-template-loader')

    await aurelia.start().then(() => {
        aurelia.setRoot('app');
    });

  // if you would like your website to work offline (Service Worker), 
  // install and enable the @easy-webpack/config-offline package in webpack.config.js and uncomment the following code:
  /*
  const offline = await System.import('offline-plugin/runtime');
  offline.install();
  */
}