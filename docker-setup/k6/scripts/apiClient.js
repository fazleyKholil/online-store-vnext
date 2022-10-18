import http from "k6/http";
import config from "./config.js";

module.exports = {
    post,
    healthcheck
};

var defaultHeaders = {
    "Content-Type": "application/json"
};

const url = config.baseUrl + "/OnlineStore/order";
const healthUrl = config.baseUrl + "/_system/ping";

function post(request) {

    const payload = JSON.stringify(request);

    const params = { 
        headers: defaultHeaders,
        tags: { name: "process" }
    };

    return http.post(url, payload, params);
}

function healthcheck() {
    return http.get(healthUrl);
}