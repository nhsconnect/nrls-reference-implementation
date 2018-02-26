export interface IPatient {
    identifier: IIdentifier[];
    activeElement: IActiveElement;
    name: INameElement[];
    telecom: IPatientTelecom[];
    genderElement: IAddressTypeElement;
    birthDateElement: IPatientBirthDateElement;
    deceased: IActiveElement;
    address: IAddressElement[];
    maritalStatus: null;
    multipleBirth: null;
    photo: any[];
    contact: IContact[];
    animal: null;
    communication: any[];
    generalPractitioner: any[];
    managingOrganization: IManagingOrganization;
    link: any[];
    text: Text;
    contained: any[];
    extension: any[];
    modifierExtension: any[];
    idElement: ICityElement;
    meta: IMeta;
    implicitRulesElement: null;
    languageElement: null;
}

export interface IActiveElement {
    value: boolean;
    elementId: null;
    extension: any[];
}

export interface IAddressElement {
    useElement: IAddressTypeElement;
    typeElement: IAddressTypeElement;
    textElement: ICityElement;
    lineElement: ICityElement[];
    ICityElement: ICityElement;
    districtElement: ICityElement;
    stateElement: ICityElement;
    postalCodeElement: ICityElement;
    countryElement: null;
    period: IAddressPeriod;
    elementId: null;
    extension: any[];
}

export interface ICityElement {
    value: string;
    elementId: null;
    extension: any[];
}

export interface IAddressPeriod {
    startElement: ICityElement;
    endElement: null;
    elementId: null;
    extension: any[];
}

export interface IAddressTypeElement {
    value: number;
    elementId: null;
    extension: any[];
}

export interface IPatientBirthDateElement {
    value: string;
    elementId: null;
    extension: IBirthDateElementExtension[];
}

export interface IBirthDateElementExtension {
    url: string;
    value: ICityElement;
    elementId: null;
    extension: any[];
}

export interface IContact {
    relationship: IRelationship[];
    name: IContactName;
    telecom: IContactTelecom[];
    address: IContactAddress;
    genderElement: IAddressTypeElement;
    organization: null;
    period: IAddressPeriod;
    modifierExtension: any[];
    elementId: null;
    extension: any[];
}

export interface IContactAddress {
    useElement: IAddressTypeElement;
    typeElement: IAddressTypeElement;
    textElement: null;
    lineElement: ICityElement[];
    ICityElement: ICityElement;
    districtElement: ICityElement;
    stateElement: ICityElement;
    postalCodeElement: ICityElement;
    countryElement: null;
    period: IAddressPeriod;
    elementId: null;
    extension: any[];
}

export interface IContactName {
    useElement: null;
    textElement: null;
    familyElement: IPatientBirthDateElement;
    givenElement: ICityElement[];
    prefixElement: any[];
    suffixElement: any[];
    period: null;
    elementId: null;
    extension: any[];
}

export interface IRelationship {
    coding: ICoding[];
    textElement: null;
    elementId: null;
    extension: any[];
}

export interface ICoding {
    systemElement: ICityElement;
    versionElement: null;
    codeElement: ICityElement;
    displayElement: null;
    userSelectedElement: null;
    elementId: null;
    extension: any[];
}

export interface IContactTelecom {
    systemElement: IAddressTypeElement;
    valueElement: ICityElement;
    useElement: null;
    rankElement: null;
    period: null;
    elementId: null;
    extension: any[];
}

export interface IIdentifier {
    useElement?: IAddressTypeElement;
    type?: IRelationship;
    systemElement: ICityElement;
    valueElement: ICityElement;
    period?: IAddressPeriod;
    assigner?: IAssigner;
    elementId: null;
    extension: IIdentifierExtension[];
}

export interface IAssigner {
    referenceElement: null;
    identifier: null;
    displayElement: ICityElement;
    elementId: null;
    extension: any[];
}

export interface IIdentifierExtension {
    url: string;
    value: IRelationship;
    elementId: null;
    extension: any[];
}

export interface IManagingOrganization {
    referenceElement: ICityElement;
    identifier: null;
    displayElement: null;
    elementId: null;
    extension: any[];
}

export interface IMeta {
    versionIdElement: ICityElement;
    lastUpdatedElement: ICityElement;
    profileElement: any[];
    security: any[];
    tag: any[];
    elementId: null;
    extension: any[];
}

export interface INameElement {
    useElement: IAddressTypeElement;
    textElement: null;
    familyElement?: ICityElement;
    givenElement: ICityElement[];
    prefixElement: any[];
    suffixElement: any[];
    period?: INamePeriod;
    elementId: null;
    extension: any[];
}

export interface INamePeriod {
    startElement: null;
    endElement: ICityElement;
    elementId: null;
    extension: any[];
}

export interface IPatientTelecom {
    systemElement?: IAddressTypeElement;
    valueElement?: ICityElement;
    useElement: IAddressTypeElement;
    rankElement?: IAddressTypeElement;
    period?: INamePeriod;
    elementId: null;
    extension: any[];
}

export interface IText {
    statusElement: IAddressTypeElement;
    div: string;
    elementId: null;
    extension: any[];
}
