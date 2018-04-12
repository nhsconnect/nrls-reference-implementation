import { IWebAppConfig } from "../interfaces/IWebAppConfig";

export class ConfigSvc {

    private static _config: IWebAppConfig;

    constructor() {

        if (!ConfigSvc._config) {
            $.ajax({
                url: '/webappconfig.json',
                success: (data) => {
                    ConfigSvc._config = data;
                },
                async : false
            });
        }
    }

    get config(): IWebAppConfig {
        return ConfigSvc._config;
    }
}