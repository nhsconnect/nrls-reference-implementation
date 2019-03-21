export class StringHelper {

    static cleanInput(text?: string): string {

        if (!text) {
            return "";
        }

        return text.replace(/[^A-Za-z0-9_\-/\s;:.@#~£$&%!'\+\(\)\[\]\*\?,>]/g, "");
    }


    static convertDataURIToBinary(dataURI): Uint8Array {

        let BASE64_MARKER = ';base64,';
        let base64Index = dataURI.indexOf(BASE64_MARKER) + BASE64_MARKER.length;
        let base64 = dataURI.substring(base64Index);
        let raw = window.atob(base64);
        let rawLength = raw.length;
        let array = new Uint8Array(new ArrayBuffer(rawLength));

        for (var i = 0; i < rawLength; i++) {
            array[i] = raw.charCodeAt(i);
        }
        return array;
    }
}