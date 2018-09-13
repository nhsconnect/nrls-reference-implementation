export class AnalyticsSvc {

    canTrack: boolean = false;

    constructor() {

    }

    trackPage(titleSlice?: string, altPath?: string) {
        if (!this.canTrack) {
            return false;
        }

        let pageTitle = this.getPageTitle(titleSlice);
        let pageLoadTime = this.getLoadTime();
        let pagePath = altPath || window.location.pathname;

        window['ga']('set', { page: pagePath, title: pageTitle, anonymizeIp: true });
        window['ga']('send', 'pageview');

        if (pageLoadTime) {
            window['ga']('send', 'timing', 'Page Load Time', 'load', pageLoadTime);
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
        if (!this.canTrack) {
            return false;
        }

        window['ga']('set', { anonymizeIp: true });
        window['ga']('send', 'event', category, action, label, value);
    }

    start(canStart: boolean, callback: () => void) {
        this.canTrack = canStart;

        if (typeof callback === "function") {
            callback();
        }
    }

    stop(callback: () => void) {
        this.canTrack = false;

        if (typeof callback === "function") {
            callback();
        }
    }

    private getPageTitle(titleSlice?: string) {
        let pageTitle = document.title;

        if (titleSlice) {
            pageTitle = document.title = this.updateTitleSlices(pageTitle, titleSlice.replace("-", " "));
        }

        return pageTitle;
    }

    private updateTitleSlices(title: string, addition: string): string {

        if (title) {
            let titleBreak = title.split("-");

            if (titleBreak.length > 0) {

                if (addition) {
                    titleBreak.splice(1, 0, addition);
                }

                return titleBreak.join(" - ");

            } else if (addition) {
                return addition;
            }
        }

        return "";
    }

    startLoadTime() {
        window['pageStartTime'] = window['pageInit'] || Date.now();
    }

    stopLoadTime() {
        window['pageEndTime'] = Date.now();
    }

    private getLoadTime() {

        let pageLoadTime: number | undefined = (window['pageEndTime'] && window['pageStartTime']) ? (window['pageEndTime'] - window['pageStartTime']) / 1000 : undefined;

        return pageLoadTime;
    }

    clearInitTime() {
        window['pageInit'] = undefined;
    }

}