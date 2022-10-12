import { sleep } from "k6";
import http from "k6/http";

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages:[
        {duration:'2m', target: 400},
        {duration:'3h56m', target: 400},
        {duration:'4m', target: 0},
    ]
};

const API_BASE_URL = "http://localhost:5243/api/"

export default () => {

    const res = http.batch([
        [ "GET", `${API_BASE_URL}/Beacons?Page=1&Size=100`],
    ]);

    sleep(1);
};