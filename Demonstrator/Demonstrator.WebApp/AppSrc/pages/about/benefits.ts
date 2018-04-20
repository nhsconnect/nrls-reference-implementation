import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";

export class AboutBenefits {
    heading: string = 'The Benefits of the NRLS';
    breadcrumb: Array<IBreadcrumb> = [];

    created() {
        this.setBreadcrumb();
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome' });
        this.breadcrumb.push(<IBreadcrumb>{ title: 'About', route: 'about', isActive: false, isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'about-benefits', isActive: true });
    }

}


