import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";

export class CookiePolicy {
    heading: string = 'Cookie Policy';
    breadcrumb: Array<IBreadcrumb> = [];

    created() {
        this.setBreadcrumb();
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'cookie-policy', isActive: true });
    }
}


