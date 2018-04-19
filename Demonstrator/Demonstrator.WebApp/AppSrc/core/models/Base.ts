import { StringHelper } from "../helpers/converters/string";


export class BaseModel {

    cleanContent(content?: string) {
        return StringHelper.cleanInput(content);
    }
}