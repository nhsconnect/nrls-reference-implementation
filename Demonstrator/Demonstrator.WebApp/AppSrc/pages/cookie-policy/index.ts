import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";
import { CookieSvc } from "../../core/services/CookieService";
import { inject } from "aurelia-framework";

@inject(CookieSvc)
export class CookiePolicy {
    heading: string = 'Cookie Policy';
    breadcrumb: Array<IBreadcrumb> = [];

    constructor(private cookieSvc: CookieSvc) {

    }

    created() {
        this.setBreadcrumb();
    }

    attached() {
        this.cookieSvc.showDeclaration("cookieDeclarationWrapper");
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'cookie-policy', isActive: true });
    }
}


