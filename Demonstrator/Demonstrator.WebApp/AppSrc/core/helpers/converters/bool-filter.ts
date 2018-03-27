export class BoolFilterValueConverter {
    toView(items: any, key: string, value: boolean) {

        if (!items || !key || !value || key === "") return items;

        return items.filter((item) => item[key] === value || (!value && item[key] !== value));
    }
}