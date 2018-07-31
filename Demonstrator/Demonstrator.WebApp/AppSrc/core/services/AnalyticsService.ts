export class AnalyticsSvc {

    _ga: any;

    constructor() {
        this._ga = window['ga'];
    }

    trackPage(path: string, title: string, loadTime?: number) {

        console.log("page: " + path, "title: " + title, "loaded: " + loadTime);

        if (!this._ga) {
            return false;
        }

        this._ga('set', { page: path, title: title, anonymizeIp: true });
        this._ga('send', 'pageview');

        if (loadTime) {
            this._ga('send', 'timing', 'Page Load Time', 'load', loadTime);
        }
    }

    benefitsButton(whos: string) {
        this.trackEvent('Widgets', 'View', 'Benefits', whos);
    }

    scenarioContext(whos: string) {
        this.trackEvent('Widgets', 'View', 'Scenario Context', whos);
    }

    downloads(which: string) {
        this.trackEvent('Resources', 'Clicked', 'Downloads', which);
    }

    genericSystems(system: string, whos: string) {
        this.trackEvent('Systems', 'View', system, whos);
    }

    contactsModal(from: string) {
        this.trackEvent('Widgets', 'View', "Contact Us", from);
    }

    trackEvent(category: string, action: string, label: string, value: string) {
        console.log('send', 'event', category, action, label, value);
        if (!this._ga) {
            return false;
        }

        this._ga('set', { anonymizeIp: true });
        this._ga('send', 'event', category, action, label, value);
    }

}