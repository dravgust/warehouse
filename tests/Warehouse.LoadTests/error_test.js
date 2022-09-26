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

export let options = {
    insecureSkipTLSVerify: true,
    vus: 10,
    duration: '1m',
};

export default () => {
    const url = "http://localhost:5243/api/Beacons/set";
    const payload = JSON.stringify({
        macAddress: "",
    });

    const params = {
        headers: {
            "Content-Type": "application/json",
        },
    };

    const res = http.post(url, payload, params);

    check(res, { "is status 400" : (r) => r.status === 400 });
};