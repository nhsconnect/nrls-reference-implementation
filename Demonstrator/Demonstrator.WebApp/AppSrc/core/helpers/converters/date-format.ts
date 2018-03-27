import * as moment  from 'moment';

export class DateFormatValueConverter {
    toView(value, inFormat, outFormat) {
        return moment(value, inFormat).isValid() ? moment(value, inFormat).format(outFormat) : "--";
    }
}