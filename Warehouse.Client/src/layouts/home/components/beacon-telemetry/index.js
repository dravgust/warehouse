import PropTypes from "prop-types";
import { useQuery } from "react-query";
import { fetchBeaconTelemetry } from "utils/query-keys";
import { getBeaconTelemetry } from "services/warehouse-service";
import Grid from "@mui/material/Grid";
import Beacon from "../beacon";
import DefaultInfoCard from "examples/Cards/InfoCards/DefaultInfoCard";

const BeaconTelemetry = ({ item }) => {
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchBeaconTelemetry, item.macAddress], getBeaconTelemetry, {
    refetchInterval: 1000 * 10,
    refetchIntervalInBackground: false,
  });
  return (
    <Grid container spacing={3}>
      <Grid item xs={12}>
        <Beacon
          macAddress={item.macAddress}
          name={item.name ? item.name : "n/a"}
          lastUpdate={response && response.receivedAt}
        />
      </Grid>

      <Grid item xs={12} md={6}>
        <DefaultInfoCard
          icon="thermostat"
          title="Temperature"
          description="Ambient Temperature"
          value={isSuccess && response.temperature ? `${response.temperature.toFixed()}C°` : "--"}
          isLoading={isLoading}
        />
      </Grid>
      <Grid item xs={12} md={6}>
        <DefaultInfoCard
          icon="waves"
          title="Humidity"
          description="Absolute Humidity"
          value={isSuccess && response.humidity ? `${response.humidity.toFixed()}%` : "--"}
          isLoading={isLoading}
        />
      </Grid>
      <Grid item xs={12} md={6}>
        <DefaultInfoCard
          icon="battery_full"
          title="Battery"
          description="Battery Level"
          value={isSuccess && response.battery ? `${response.battery}mV` : "--"}
          isLoading={isLoading}
        />
      </Grid>
      <Grid item xs={12} md={6}>
        <DefaultInfoCard
          icon="360"
          title="Acceleration"
          description="Acceleration Value"
          value={
            isSuccess && response.x0 ? `:${response.x0} :${response.y0} :${response.z0}` : "--"
          }
          isLoading={isLoading}
        />
      </Grid>
    </Grid>
  );
};
export default BeaconTelemetry;

BeaconTelemetry.prototype = {
  item: PropTypes.object.isRequired,
};
