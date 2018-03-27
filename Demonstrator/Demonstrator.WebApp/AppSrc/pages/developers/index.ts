import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";

export class Developers {
    heading: string = 'Developers';
    breadcrumb: Array<IBreadcrumb> = [];

    created() {
        this.setBreadcrumb();
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'developers', isActive: true });
    }
}


