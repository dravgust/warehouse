//Ubuntu
//sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
// echo "deb https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
//
// # update repo and install
// sudo apt-get update
// sudo apt-get install k6

//alias k6="cmd.exe /C k6"

//Windows
//choco install k6

import { check } from "k6";
import http from "k6/http";

const BASE_URL = "http://localhost:5243";
const SLEEP_DURATION = 1;
let xApiVersion = "1";

export let options = {
    insecureSkipTLSVerify: true,
    vus: 10,
    duration: '1m',
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


export default (data) => {

    if (!data.token) {
        throw new Error('invalid token: ' + JSON.stringify(data));
    }

    let params = {headers: {"x-api-version": `${xApiVersion}`, "Accept": "application/json", "Content-Type": "application/json", "Authorization": `Basic ${data.token}`}};

    const url = BASE_URL + "/api/Beacons/set";
    const payload = JSON.stringify({
        macAddress: "",
    });

    const res = http.post(url, payload, params);

    check(res, { "is status 400" : (r) => r.status === 400 });
};