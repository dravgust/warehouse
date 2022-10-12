import { sleep } from "k6";
import http from "k6/http";

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages:[
        { duration:'5m', target:100 }, //move from 1 to 100 requests over 5 mins
        { duration:'10m', target:100 },//stay at 100 requests for 10 mins
        { duration:'5m', target:0 },   // ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ["p(99)<150"] //99% of request must complete below 150 ms
    }
};

const API_BASE_URL = "http://localhost:5243/api/"

export default () => {
    const url = "http://localhost:5243/api/Beacons";
    const res = http.get(url);

    sleep(1);
};