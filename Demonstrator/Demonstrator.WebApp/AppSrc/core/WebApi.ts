import { inject } from 'aurelia-framework';
import { fetch } from 'whatwg-fetch';
import { HttpClient, json } from 'aurelia-fetch-client';
import { EventAggregator } from 'aurelia-event-aggregator';
import { DialogRequested } from './helpers/EventMessages';

interface ApiResponse {
    Message: string;
    Success: boolean;
    Data: Object;
}

@inject(EventAggregator)
export class WebAPI {

    constructor(private ea: EventAggregator) {}

    httpClient = new HttpClient();
    apiResponse: ApiResponse;

    get isRequesting() {
        return this.httpClient.isRequesting;
    }

    getHeaders() {
        var headers = {
            'Accept': 'application/json',
            'X-Requested-With': 'Fetch'
        };

        return headers;
    }

    do<T>(url, body, type) {
        this.httpClient.configure(config => {
            config
                .useStandardConfiguration()
                //Running on same host:port as API so base url can be left as is
                .withBaseUrl('/api/') 
                .withDefaults({
                    credentials: 'same-origin',
                    headers: this.getHeaders(),
                    mode: 'same-origin'
                })
                .withInterceptor({
                    request(request) {
                        //let authHeader = fakeAuthService.getAuthHeaderValue(request.url);
                        //request.headers.append('Authorization', authHeader);
                        //console.log(`Requesting ${request.method} ${request.url}`);
                        return request;
                    },
                    response(response) {
                        //console.log(`Received ${response.status} ${response.url}`);
                        return response;
                    }
                });
        });


        let reqBody = (type === "get") ? {} : {
            method: type,
            body: json(body)
        };

        let that = this;
        return new Promise<T>(function (resolve, reject) {
            that.httpClient
                .fetch(url, reqBody)
                .then(response => {
                    let rsv = resolve(response.json());
                    return rsv;
                }, error => {
                    if (!error.ok && error.status === 404) {
                        that.ea.publish(new DialogRequested({ Details: error.statusText }));
                    } else {
                        error.json().then(serverError => {
                            that.ea.publish(new DialogRequested(serverError));
                        });
                    }
                });
        });
    }

}
