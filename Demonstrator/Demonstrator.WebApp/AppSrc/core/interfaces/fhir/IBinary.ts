export interface IBinary {
    contentType: string;
    content: string;
    contentElement: IBinaryContentElement;
}

export interface IBinaryContentElement {
    value: string;
}