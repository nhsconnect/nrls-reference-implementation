import { bindable, bindingMode, inject } from 'aurelia-framework';
import { BenefitsSvc } from '../../services/BenefitsService';
import { IBenefitDialog } from '../../interfaces/IBenefitDialog';
import { IBenefitMenu } from '../../interfaces/IBenefitMenu';

@inject(BenefitsSvc)
export class benefitsdialog {
    @bindable({ defaultBindingMode: bindingMode.oneWay }) withmenu: boolean;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) ispopup: boolean;
    @bindable({ defaultBindingMode: bindingMode.oneWay }) bindtomenu: boolean;
    @bindable benefitfor: string;
    @bindable benefitforid: string;
    benefitdialog: IBenefitDialog;
    benefitMenu: IBenefitMenu;
    activeBenefit: string;
    currentBenefit: number = 0;
    benefitTypeClass: string = "";
    benefitsLoading: boolean = false;
    menuLoading: boolean = false;

    constructor(private benefitsSvc: BenefitsSvc) { }

    created() {

        //$(function () {
        //    $('[data-toggle="tooltip"]').tooltip();
        //});
    }

    attached() {

        if (!this.bindtomenu) {
            this.loadBenefit();
        } 

        if (this.withmenu) {
            this.getMenu();
        }
    }

    getBenefit(benefitfor: string, benefitforid: string) {

        this.benefitfor = benefitfor;
        this.benefitforid = benefitforid;

        this.loadBenefit();
    }

    loadBenefit() {
        this.benefitsLoading = true;

        window.setTimeout(() => {
            this.benefitsSvc.getFor(this.benefitfor, this.benefitforid).then(benefitdialog => {
                this.benefitdialog = benefitdialog;
                this.benefitsLoading = false;
                this.setBenefits();
            });
        }, 250); //Looks better with delay :)

    }

    changeType(category: string) {
        this.activeBenefit = category;
        this.currentBenefit = 0;
    }

    prevBenefit() {
        this.currentBenefit = (this.currentBenefit === 0) ? 0 : this.currentBenefit - 1;
    }

    nxtBenefit() {
        let totalBenefits = this.benefitdialog.benefits[this.activeBenefit].length;
        this.currentBenefit = (this.currentBenefit === totalBenefits - 1) ? totalBenefits - 1 : this.currentBenefit + 1;
    }

    showBenefit(index: number) {
        this.currentBenefit = index;
    }

    private getMenu() {
        this.menuLoading = true;

        if (this.bindtomenu) {
            this.benefitsLoading = true;
        }

        this.benefitsSvc.getMenu().then(benefitMenu => {
            this.benefitMenu = benefitMenu;
            this.menuLoading = false;

            if (this.bindtomenu) {
                let firstitem = this.benefitMenu.menuItems[0];

                this.benefitfor = firstitem.type;
                this.benefitforid = firstitem.id;

                this.loadBenefit();
            }
        });
    }

    private setBenefits(): void {

        if (this.benefitdialog) {
            this.benefitdialog.categories.sort();
            this.activeBenefit = this.benefitdialog.categories.length > 0 ? this.benefitdialog.categories[0] : "";
            this.benefitTypeClass = `b-type-link-${this.benefitdialog.totalCategories}`;
        }
    }

}