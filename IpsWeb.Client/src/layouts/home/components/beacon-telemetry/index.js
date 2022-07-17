import PropTypes from "prop-types";
import { useQuery } from "react-query";
import { fetchBeaconTelemetry } from "../../../../utils/query-keys";
import { getBeaconTelemetry } from "../../../../services/warehouse-service";
import Grid from "@mui/material/Grid";
import Beacon from "../beacon";
import DefaultInfoCard from "../../../../examples/Cards/InfoCards/DefaultInfoCard";

const BeaconTelemetry = ({ item }) => {
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchBeaconTelemetry, item.macAddress], getBeaconTelemetry);
  return (
    <Grid container spacing={3}>
      <Grid item xs={12}>
        <Beacon macAddress={item.macAddress} name={item.name} />
      </Grid>

      {isSuccess && (
        <>
          <Grid item xs={12} md={6}>
            <DefaultInfoCard
              icon="thermostat"
              title="Temperature"
              description="Ambient Temperature"
              value={response.temperature ? `${response.temperature}C&deg` : "--"}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <DefaultInfoCard
              icon="waves"
              title="Humidity"
              description="Absolute humidity"
              value={response.humidity ? `${response.humidity}%` : "--"}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <DefaultInfoCard
              icon="battery_full"
              title="Battery"
              description="Battery level"
              value={response.battery ? `${response.battery}%` : "--"}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <DefaultInfoCard
              icon="location_off"
              title="Location"
              description="Beacon location"
              value="--"
            />
          </Grid>
        </>
      )}
    </Grid>
  );
};
export default BeaconTelemetry;

BeaconTelemetry.prototype = {
  item: PropTypes.object.isRequired,
};
