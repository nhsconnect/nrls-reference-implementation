import { IBreadcrumb } from "../../core/interfaces/IBreadcrumb";

export class About {
    heading: string = 'About the NRLS';
    breadcrumb: Array<IBreadcrumb> = [];
    benefitfor: string = "ActorOrganisation";
    benefitforid: string = "5a8daa5c952e372cbdb317ea";

    created() {
        this.setBreadcrumb();
    }

    private setBreadcrumb(): void {
        this.breadcrumb.push(<IBreadcrumb>{ title: 'Home', route: 'welcome', isBack: true });
        this.breadcrumb.push(<IBreadcrumb>{ title: this.heading, route: 'about', isActive: true });
    }

}


