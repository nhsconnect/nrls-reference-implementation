import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";

export class Accessibility {
    heading: string = 'Accessibility';
    breadcrumb: Array<IBreadcrumb> = [];

    created() {
        this.setBreadcrumb();
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'accessibility', isActive: true });
    }
}


