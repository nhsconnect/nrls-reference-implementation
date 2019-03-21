import { IDemonstratorConfig } from "./IDemonstratorConfig";
import { IWebAppConfig } from "./IWebAppConfig";

export interface IConfigSvc {
    webAppConfig: IWebAppConfig;
    demoAppConfig: IDemonstratorConfig;
}