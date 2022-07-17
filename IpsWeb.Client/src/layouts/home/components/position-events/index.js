import { useState } from "react";

// @mui material components
import Card from "@mui/material/Card";
import Icon from "@mui/material/Icon";
import { IconButton, Tooltip } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import TimelineItem from "examples/Timeline/TimelineItem";
import { useQuery } from "react-query";
import { formatDistance } from "date-fns";
import Stack from "@mui/material/Stack";
import Pagination from "@mui/material/Pagination";
import React from "react";
import { fetchEvents } from "utils/query-keys";
import { getEvents } from "services/warehouse-service";

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
          response.data.map((item) => (
            <TimelineItem
              key={item.id}
              color={item.event === "IN" ? "success" : "error"}
              icon={item.event == "IN" ? "location_on" : "location_off"}
              title={
                <SuiTypography variant="caption" fontWeight="medium">
                  The device {item.macAddress} is {item.event === "OUT" ? "out of the" : "entered"}{" "}
                  the <span style={{ color: "#17c1e8" }}>{item.siteName}</span>
                </SuiTypography>
              }
              dateTime={formatDistance(new Date(item.timeStamp), new Date(), { addSuffix: true })}
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
