import { IDemonstratorConfig } from "../interfaces/IDemonstratorConfig";

export class DemonstratorConfig implements IDemonstratorConfig {

    public appVersion: string;
    public updatedAt: string;
    public baseUrl: string;
    public gaTrackId: string;
    public cookieBotId: string;

    constructor() {    }
}