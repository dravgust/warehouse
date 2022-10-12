import { sleep } from "k6";
import http from "k6/http";

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages:[
        { duration:'1m', target:100 },
        { duration:'2m', target:50 },
        { duration:'4m', target:1000 },//spike the request to 1000 at 4 mins
        { duration:'8m', target:1000 },// stay at 1000 request for 8 mins
        { duration:'2m', target:200 },
        { duration:'4m', target:0 },
    ]
};

const API_BASE_URL = "http://localhost:5243/api/"

export default () => {

    const res = http.batch([
        [ "GET", `${API_BASE_URL}/Beacons?Page=1&Size=100`],
    ]);

    sleep(1);
};