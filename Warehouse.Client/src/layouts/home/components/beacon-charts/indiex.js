import { useState, useEffect } from "react";
import { useQuery } from "react-query";
import { fetchBeaconTelemetryCharts } from "../../../../utils/query-keys";
import { getBeaconTelemetryCharts } from "../../../../services/warehouse-service";
import PropTypes from "prop-types";
import { format } from "date-fns";
import { Card, Icon, Stack } from "@mui/material";
import GradientLineChart from "../../../../examples/Charts/LineCharts/GradientLineChart";
import SuiBox from "../../../../components/SuiBox";
import SuiTypography from "../../../../components/SuiTypography";
import { ScaleLoader } from "react-spinners";

const BeaconTelemetryCharts = ({ item }) => {
  const {
    data: response,
    isSuccess,
    isLoading,
  } = useQuery([fetchBeaconTelemetryCharts, item.macAddress], getBeaconTelemetryCharts, {
    keepPreviousData: false,
  });

  const [temperatureChart, setTemperatureChartData] = useState({
    labels: [],
    datasets: [],
  });
  const [humidityChart, setHumidityChartData] = useState({
    labels: [],
    datasets: [],
  });
  const [isChartLoading, setIsChartLoading] = useState(true);

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
        color: "dark",
        data: humidity,
      });
      setHumidityChartData(humidityChartData);
      setIsChartLoading(false);
    }
    isSuccess && prepareData();
  }, [isSuccess, item]);

  return (
    <Stack spacing={3}>
      <Card>
        <SuiBox p={2}>
          <SuiBox mb={1} display="flex" alignItems="center">
            <Icon>thermostat</Icon>
            <SuiTypography variant="h6">&nbsp;Temperature Overview</SuiTypography>
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
          <ScaleLoader
            loading={isLoading || isChartLoading}
            color={"#17c1e8"}
            height={"100px"}
            cssOverride={{ position: "absolute", display: "inherit", top: "50%", left: "50%" }}
          />
          <GradientLineChart chart={temperatureChart} />
        </SuiBox>
      </Card>

      <Card>
        <SuiBox p={2}>
          <SuiBox mb={1} display="flex" alignItems="center">
            <Icon>waves</Icon>
            <SuiTypography variant="h6">&nbsp;Humidity Overview</SuiTypography>
          </SuiBox>
          <ScaleLoader
            loading={isLoading || isChartLoading}
            color={"#17c1e8"}
            height={"100px"}
            cssOverride={{ position: "absolute", display: "inherit", top: "50%", left: "50%" }}
          />
          <GradientLineChart chart={humidityChart} />
        </SuiBox>
      </Card>
    </Stack>
  );
};
export default BeaconTelemetryCharts;

BeaconTelemetryCharts.prototype = {
  item: PropTypes.object.isRequired,
};
