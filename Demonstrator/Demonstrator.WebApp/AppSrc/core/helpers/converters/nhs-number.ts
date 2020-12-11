export class NhsNumberValueConverter {
  toView(nhsNumber?: string) {
    return nhsNumber
      ? [nhsNumber.slice(0, 3), nhsNumber.slice(3, 6), nhsNumber.slice(6)].join(
          " "
        )
      : "--";
  }
}