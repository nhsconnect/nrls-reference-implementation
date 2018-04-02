export interface IBenefitMenu {
    menuItems: Array<IBenefitMenuItem>;
}

export interface IBenefitMenuItem {
    id: string;
    title: string;
    type: string;
    children: Array<IBenefitMenuItem>;
}





        //public IDictionary < string, IList < BenefitViewModel >> Benefits { get; set; }

