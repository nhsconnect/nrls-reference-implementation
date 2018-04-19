export class StringHelper {

    static cleanInput(text?: string): string {

        if (!text) {
            return "";
        }

        return text.replace(/[^A-Za-z0-9_\-/\s;:.@#~£$&%!'\+\(\)\[\]\*\?,>]/g, "");
    }
}