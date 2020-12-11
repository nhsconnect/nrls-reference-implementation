import * as moment  from 'moment';

export class DateFormatValueConverter {
    toView(value, inFormat, outFormat) {
        return moment(value, inFormat).isValid() ? moment(value, inFormat).format(outFormat) : "--";
    }
}

export class AgeFormatValueConverter {
  toView(value, inFormat) {
    return moment(value, inFormat).isValid()
      ? `${Math.floor(
          moment
            .duration(moment().diff(moment(value)), "ms")
            .asYears()
        )} years old`
      : "--";
  }
}
