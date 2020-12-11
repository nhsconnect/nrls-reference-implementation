import { IAddress } from "../../interfaces/fhir/IAddress";
import { IName } from "../../interfaces/fhir/IName";

export class AddressFormatValueConverter {
  toView(address?: IAddress) {
    return address
      ? [
          ...address.line,
          address.city,
          address.district,
          address.postalCode,
        ].join(", ")
      : "--";
  }
}

export class NameFormatValueConverter {
  toView(name?: IName) {
    return name
      ? [name.given.join(" "), name.family].filter(Boolean).join(" ")
      : "--";
  }
}

export class PluralizeValueConverter {
  toView(num: number, sg: string, pl: string) {
    return (num === 1 ? sg : pl).replace('$1', num.toString());
  }
}

export class BlankIfNullishValueConverter {
  toView(str) {
    return str == null ? "--" : str;
  }
}

export class JsonSerializeValueConverter {
  toView(str) {
    return JSON.stringify(str);
  }
}
