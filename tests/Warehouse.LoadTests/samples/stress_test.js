import { sleep } from "k6";
import http from "k6/http";

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages:[
        {duration:'2m', target:10},
        {duration:'4m', target:50},//normal request load
        {duration:'4m', target:1000},// request load around the breaking point
        {duration:'4m', target:1000},
        {duration:'2m', target:2000},// request load beyond the breaking point
        {duration:'4m', target:0},//scale down to 0 requests for recovery stage
    ]
};

const API_BASE_URL = "http://localhost:5243/api/"

export default () => {

    const res = http.batch([
        [ "GET", `${API_BASE_URL}/Beacons?Page=1&Size=100`],
    ]);

    sleep(1);
};