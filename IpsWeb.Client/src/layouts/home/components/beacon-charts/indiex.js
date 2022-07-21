import { useState, useEffect } from "react";
import { useQuery } from "react-query";
import { fetchBeaconTelemetryCharts } from "../../../../utils/query-keys";
import { getBeaconTelemetryCharts } from "../../../../services/warehouse-service";
import PropTypes from "prop-types";
import { format } from "date-fns";
import { Card, Stack } from "@mui/material";
import GradientLineChart from "../../../../examples/Charts/LineCharts/GradientLineChart";
import Fade from "@mui/material/Fade";
import SuiBox from "../../../../components/SuiBox";
import SuiTypography from "../../../../components/SuiTypography";
import { CircularProgress } from "@mui/material";

const BeaconTelemetryCharts = ({ item }) => {
  const { data: response, isSuccess } = useQuery(
    [fetchBeaconTelemetryCharts, item.macAddress],
    getBeaconTelemetryCharts,
    { keepPreviousData: false }
  );

  const [temperatureChart, setTemperatureChartData] = useState(null);
  const [humidityChart, setHumidityChartData] = useState(null);

  useEffect(() => {
    function prepareData() {
      const temperatureChartData = {
        labels: [],
        datasets: [],
      };
      const temperature = [];
      Object.keys(response.temperature).map(function (key, index) {
        temperatureChartData.labels.push(format(new Date(key), "HH:mm"));
        temperature.push(response.temperature[key]);
      });
      temperatureChartData.datasets.push({
        label: " Temperature C° ",
        color: "info",
        data: temperature,
      });
      setTemperatureChartData(temperatureChartData);

      const humidityChartData = {
        labels: [],
        datasets: [],
      };
      const humidity = [];
      Object.keys(response.humidity).map(function (key, index) {
        humidityChartData.labels.push(format(new Date(key), "HH:mm"));
        humidity.push(response.humidity[key]);
      });
      humidityChartData.datasets.push({
        label: " Humidity % ",
        color: "error",
        data: humidity,
      });
      setHumidityChartData(humidityChartData);
    }
    isSuccess && prepareData();
  }, [isSuccess, item]);

  return (
    <Stack spacing={3}>
      <Card>
        {Boolean(temperatureChart) ? (
          <SuiBox p={2}>
            <SuiBox mb={1}>
              <SuiTypography variant="h6">{"Temperature Overview"}</SuiTypography>
            </SuiBox>
            <SuiBox mb={2}>
              <SuiTypography component="div" variant="button" fontWeight="regular" color="text">
                {/*<SuiBox display="flex" alignItems="center">
                <SuiBox fontSize={size.lg} color="success" mb={0.3} mr={0.5} lineHeight={0}>
                  <Icon className="font-bold">arrow_upward</Icon>
                </SuiBox>
                <SuiTypography variant="button" color="text" fontWeight="medium">
                  -% more{" "}
                  <SuiTypography variant="button" color="text" fontWeight="regular">
                    in ----
                  </SuiTypography>
                </SuiTypography>
              </SuiBox>*/}
                <div></div>
              </SuiTypography>
            </SuiBox>
            <GradientLineChart height="20.25rem" chart={temperatureChart} />
          </SuiBox>
        ) : (
          <Fade
            in={!Boolean(temperatureChart)}
            style={{
              transitionDelay: !Boolean(temperatureChart) ? "800ms" : "0ms",
            }}
            unmountOnExit
          >
            <SuiBox display="flex" justifyContent="center" alignItems="center" minHeight="20.25rem">
              <CircularProgress color={"secondary"} />
            </SuiBox>
          </Fade>
        )}
      </Card>

      <Card>
        {Boolean(humidityChart) ? (
          <SuiBox p={2}>
            <SuiBox mb={1}>
              <SuiTypography variant="h6">{"Humidity Overview"}</SuiTypography>
            </SuiBox>
            <GradientLineChart height="20.25rem" chart={humidityChart} />
          </SuiBox>
        ) : (
          <Fade
            in={!Boolean(humidityChart)}
            style={{
              transitionDelay: !Boolean(humidityChart) ? "800ms" : "0ms",
            }}
            unmountOnExit
          >
            <SuiBox display="flex" justifyContent="center" alignItems="center" minHeight="20.25rem">
              <CircularProgress color={"secondary"} />
            </SuiBox>
          </Fade>
        )}
      </Card>
    </Stack>
  );
};
export default BeaconTelemetryCharts;

BeaconTelemetryCharts.prototype = {
  item: PropTypes.object.isRequired,
};
