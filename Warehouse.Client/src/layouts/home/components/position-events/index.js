import { useState } from "react";
import { IconButton, Tooltip, Card, Icon, Pagination, Stack } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import TimelineItem from "examples/Timeline/TimelineItem";
import { useQuery } from "react-query";
import { formatDistance } from "date-fns";
import React from "react";
import { fetchEvents } from "utils/query-keys";
import { getEvents } from "services/warehouse-service";

function renderEvent({ type, beacon, source, destination }) {
  let name = beacon.name ? beacon.name : beacon.macAddress;
  switch (type) {
    case 1:
      return `The ${name} entered '${destination ? destination.name : "n/a"}'`;
    case 2:
      return `The ${name} out of '${source ? source.name : "n/a"}'`;
    case 3:
      return `The ${name} moved from '${source ? source.name : "n/a"}' to '${
        destination ? destination.name : "n/a"
      }'`;
    default:
      return "n/a";
  }
}

function PositionEvents({ searchTerm = "" }) {
  const [reload, updateReloadState] = useState(null);
  const forceUpdate = () => updateReloadState(Date.now());
  const [page, setPage] = useState(1);

  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchEvents, page, searchTerm, reload], getEvents);

  return (
    <Card className="h-100">
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={3}>
        <SuiBox pt={0} px={2}>
          <SuiTypography variant="h6" fontWeight="medium">
            Events
          </SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
          <IconButton size="xl" color="inherit" onClick={forceUpdate}>
            <Tooltip title="Reload">
              <Icon>sync</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox p={2}>
        {isSuccess &&
          response.data.map((item, index) => (
            <TimelineItem
              key={index}
              color={item.type === 1 ? "success" : item.type == 2 ? "error" : "warning"}
              icon={
                item.type === 1
                  ? "add_location_alt"
                  : item.type == 2
                  ? "location_off"
                  : "location_on"
              }
              title={
                <SuiTypography variant="caption" fontWeight="medium">
                  {renderEvent(item)}
                </SuiTypography>
              }
              dateTime={formatDistance(new Date(item.timeStamp), new Date(), {
                addSuffix: true,
              })}
            />
          ))}

        {isSuccess && page && (
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
            <Stack direction="row" spacing={2}>
              <Pagination
                count={response.totalPages}
                page={page}
                onChange={(event, value) => setPage(value)}
              />
            </Stack>
          </SuiBox>
        )}
        {isLoading && (
          <SuiTypography px={2} color="secondary">
            Loading..
          </SuiTypography>
        )}
        {error && (
          <SuiTypography px={2} color="error">
            Error occurred!
          </SuiTypography>
        )}
      </SuiBox>
    </Card>
  );
}

export default PositionEvents;
