FROM loadimpact/k6

# for local
COPY ./scripts/ /
COPY ./test-cycles/ /

ENTRYPOINT k6 run -o influxdb=http://influxdb:8086/k6 /load-test-script.js