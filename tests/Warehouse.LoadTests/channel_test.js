//alias k6="cmd.exe /C k6"


import { check } from "k6";
import http from "k6/http";

const BASE_URL = "http://localhost:5243";
const SLEEP_DURATION = 1;
let xApiVersion = "1";

export let options = {
    insecureSkipTLSVerify: true,
    vus: 1000,
    duration: '10s',
};

export default () => {

    const url = BASE_URL + "/api/Notifications/test/1/1";
    const res = http.get(url);

    check(res, { "is status 200" : (r) => r.status === 200 });
};