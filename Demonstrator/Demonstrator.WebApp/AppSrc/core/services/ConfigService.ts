import { IWebAppConfig } from "../interfaces/IWebAppConfig";
import { IDemonstratorConfig } from "../interfaces/IDemonstratorConfig";
import { DemonstratorConfig } from "../models/DemonstratorConfig";
import { IConfigSvc } from "../interfaces/IConfigSvc";

export class ConfigSvc implements IConfigSvc {

    private static _webAppConfig: IWebAppConfig;
    private static _demoAppConfig: IDemonstratorConfig;

    constructor() {

        if (!ConfigSvc._webAppConfig) {
            $.ajax({
                url: '/webappconfig.json',
                success: (data) => {
                    ConfigSvc._webAppConfig = data;
                },
                async : false
            });
        }

        if (!ConfigSvc._demoAppConfig) {
            ConfigSvc._demoAppConfig = (window['demonstratorConfig'] || new DemonstratorConfig());
        }
    }

    get webAppConfig(): IWebAppConfig {
        return ConfigSvc._webAppConfig;
    }

    get demoAppConfig(): IDemonstratorConfig {
        return ConfigSvc._demoAppConfig;
    }
}