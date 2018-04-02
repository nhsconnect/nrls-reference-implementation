export interface IBenefitDialog {
    benefitsTitle: string;
    benefitIds: Array<string>;
    categories: Array<string>;
    benefits: Array<IBenefit>;
    hasEfficiency: boolean;
    hasFinancial: boolean;
    hasHealth: boolean;
    hasSafety: boolean;
    totalCategories: number;
}

export interface IBenefit {
    id: string;
    text: string;
    categories: Array<string>;
    order: number;
    type: string;
}





        //public IDictionary < string, IList < BenefitViewModel >> Benefits { get; set; }

