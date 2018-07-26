import * as moment  from 'moment';

export class NhsNumberValueConverter {
    toView(value) {
        return !value || value.length !== 10 ? value : `${value.substr(0, 3)} ${value.substr(3, 3)} ${value.substr(6, 4)}`;
    }
}