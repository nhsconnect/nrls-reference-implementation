export class CookieSvc {

    set: boolean = false;
    cookieBotId: string;

    start(cookiebotId: string) {

        this.cookieBotId = cookiebotId;

        if (!this.set) {
            this.set = true;
        }
    }

    showDeclaration(elmId: string) {
        let script = document.createElement('script');
        script.src = `https://consent.cookiebot.com/${this.cookieBotId}/cd.js`;
        script.setAttribute("id", "CookieDeclaration");
        script.setAttribute("async", "true");

        let wrapper = document.getElementById(elmId);

        if (wrapper) {
            wrapper.appendChild(script);
        }
    }

    runScripts() : void {
        if (window['Cookiebot']) {
            window['Cookiebot'].runScripts();
        }
    }

    get canTrack () {
        return window['Cookiebot'] ? window['Cookiebot'].consent.statistics : false;
    }

    

}