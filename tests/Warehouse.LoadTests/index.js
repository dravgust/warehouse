import http from "k6/http";
import { group, check, sleep } from "k6";
import { Rate } from "k6/metrics";

const BASE_URL = "http://localhost:5243";
const SLEEP_DURATION = 3;
let xApiVersion = "1";

// A custom metric to track failure rates
var failureRate = new Rate("check_failure_rate");

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    /*stages:[
        { duration:'5m', target:100 }, //move from 1 to 100 requests over 5 mins
        { duration:'10m', target:100 },//stay at 100 requests for 10 mins
        { duration:'5m', target:0 },   // ramp-down to 0 users
    ],*/
    vus: 1,
    duration: '1m',
    thresholds: {
        http_req_duration: ["p(99)<150"] //99% of request must complete below 150 ms
    }
};

export function setup() {
    let url = BASE_URL + `/api/Account/login`;
    let body = {"email": "anton@vayosoft.com", "password": "1q2w3e4r"};
    let params = {headers: {"Content-Type": "application/json", "x-api-version": `${xApiVersion}`, "Accept": "application/json"}};
    let request = http.post(url, JSON.stringify(body), params);

    check(request, {
        "Success": (r) => r.status === 200
    });

    return request.json();
}

export default function(data) {

    if (!data.token) {
        throw new Error('invalid token: ' + JSON.stringify(data));
    }

    let params = {headers: {"x-api-version": `${xApiVersion}`, "Accept": "application/json", "Authorization": `Basic ${data.token}`}};

    group("Bootstrap", () => {

        // Request No. 1
        {
            let url = BASE_URL + `/api/Account/bootstrap`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }
    });

    let selectedSite = null;
    let selectedProduct = null;
    let selectedBeacon = null;
    group("Dashboard", () => {

        let searchTerm = '';
        let productId = '';
        let size = '10';
        let siteId = '';
        let page = '1';

        let resps = http.batch([
            { method: 'GET', url: BASE_URL + `/api/Dashboard/sites`, params: params },
            { method: 'GET', url: BASE_URL + `/api/Dashboard/products`, params: params },
            { method: 'GET', url: BASE_URL + `/api/Dashboard/beacons?SearchTerm=${searchTerm}&SiteId=${siteId}&ProductId=${productId}&Page=${page}&Size=${size}`, params: params },
            { method: 'GET', url: BASE_URL + `/api/Notifications?SearchTerm=${searchTerm}&Page=${page}&Size=${size}`, params: params },
            { method: 'GET', url: BASE_URL + `/api/Events?SearchTerm=${searchTerm}&Page=${page}&Size=${size}`, params: params },
        ]);

        const beaconsResp = resps[2].json();
        if(beaconsResp.status === 200 && beaconsResp.items.length > 0){
            selectedBeacon = beaconsResp.items[0];
        }

        failureRate.add(!check(resps, {
            "Success": (r) => r[0].status === 200 && r[1].status === 200 && r[2].status === 200,
        }));

    });

    sleep(SLEEP_DURATION)

    if(selectedBeacon) {
        group("Beacon Data", () => {
            let id = selectedBeacon.macAddress

            // Request No. 1
            {
                let url = BASE_URL + `/api/Dashboard/beacon/${id}`;
                let request = http.get(url, params);
                check(request, {
                    "Success": (r) => r.status === 200
                });
            }
            // Request No. 2
            {
                let url = BASE_URL + `/api/Dashboard/beacon/charts/${id}`;
                let request = http.get(url, params);
                check(request, {
                    "Success": (r) => r.status === 200
                });
            }
        });
    }

    sleep(SLEEP_DURATION)

    group("Products", () => {
        let searchTerm = '';
        let size = '10';
        let page = '1';

        // Request No. 1
        {
            let url = BASE_URL + `/api/Products?SearchTerm=${searchTerm}&Page=${page}&Size=${size}`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }

        // Request No. 2
        {
            let url = BASE_URL + `/api/Products/metadata`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }

        // Request No. 3
        {
            let url = BASE_URL + `/api/Products/item-metadata`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }

    });

    sleep(SLEEP_DURATION);

    group("Sites", () => {
        let searchTerm = '';
        let size = '10';
        let page = '1';

        // Request No. 1
        {
            let url = BASE_URL + `/api/Sites?SearchTerm=${searchTerm}&Page=${page}&Size=${size}`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }
    });

    sleep(SLEEP_DURATION);

    group("Beacons", () => {
        let searchTerm = '';
        let size = '10';
        let page = '1';

        {
            let url = BASE_URL + `/api/Beacons?SearchTerm=${searchTerm}&Page=${page}&Size=${size}`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }
    });

    sleep(SLEEP_DURATION);

    group("Alerts", () => {
        let searchTerm = '';
        let size = '10';
        let page = '1';

        // Request No. 1
        {
            let url = BASE_URL + `/api/Alerts?SearchTerm=${searchTerm}&Page=${page}&Size=${size}`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }
    });

    sleep(SLEEP_DURATION);

    group("Users", () => {
        let searchTerm = '';
        let size = '10';
        let page = '1';

        // Request No. 1
        {
            let url = BASE_URL + `/api/Users?page=${page}&size=${size}&searchTerm=${searchTerm}`;
            let request = http.get(url, params);

            failureRate.add(!check(request, {
                "Success": (r) => r.status === 200
            }));
        }
    });

}
