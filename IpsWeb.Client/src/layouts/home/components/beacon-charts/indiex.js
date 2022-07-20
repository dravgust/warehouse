import typography from "../../../../assets/theme/base/typography";
import { useQuery } from "react-query";
import { fetchBeaconTelemetryCharts } from "../../../../utils/query-keys";
import { getBeaconTelemetryCharts } from "../../../../services/warehouse-service";
import PropTypes from "prop-types";
import { format } from "date-fns";
import { Stack } from "@mui/material";
import GradientLineChart from "../../../../examples/Charts/LineCharts/GradientLineChart";

const BeaconTelemetryCharts = ({ item }) => {
  const { size } = typography;
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchBeaconTelemetryCharts, item.macAddress], getBeaconTelemetryCharts);

  const temperatureChartData = {
    labels: [],
    datasets: [
      {
        label: "",
        color: "info",
        data: [],
      },
    ],
  };
  const humidityChartData = {
    labels: [],
    datasets: [
      {
        label: "",
        color: "error",
        data: [],
      },
    ],
  };

  if (isSuccess) {
    temperatureChartData.labels = [];
    temperatureChartData.datasets = [];

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

    humidityChartData.labels = [];
    humidityChartData.datasets = [];
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
  }

  return (
    <Stack spacing={3}>
      <GradientLineChart
        title="Temperature Overview"
        /*description={
                    <SuiBox display="flex" alignItems="center">
                      <SuiBox fontSize={size.lg} color="success" mb={0.3} mr={0.5} lineHeight={0}>
                        <Icon className="font-bold">arrow_upward</Icon>
                      </SuiBox>
                      <SuiTypography variant="button" color="text" fontWeight="medium">
                        -% more{" "}
                        <SuiTypography variant="button" color="text" fontWeight="regular">
                          in ----
                        </SuiTypography>
                      </SuiTypography>
                    </SuiBox>
                  }*/
        height="20.25rem"
        chart={temperatureChartData}
      />

      <GradientLineChart
        title="Humidity Overview"
        /*description={
                    <SuiBox display="flex" alignItems="center">
                      <SuiBox fontSize={size.lg} color="success" mb={0.3} mr={0.5} lineHeight={0}>
                        <Icon className="font-bold">arrow_upward</Icon>
                      </SuiBox>
                      <SuiTypography variant="button" color="text" fontWeight="medium">
                        -% more{" "}
                        <SuiTypography variant="button" color="text" fontWeight="regular">
                          in ----
                        </SuiTypography>
                      </SuiTypography>
                    </SuiBox>
                  }*/
        height="20.25rem"
        chart={humidityChartData}
      />
    </Stack>
  );
};
export default BeaconTelemetryCharts;

BeaconTelemetryCharts.prototype = {
  item: PropTypes.object.isRequired,
};
