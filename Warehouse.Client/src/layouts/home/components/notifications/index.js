import { useState } from "react";
import { IconButton, Tooltip, Card, Icon, Pagination, Stack } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import TimelineItem from "examples/Timeline/TimelineItem";
import { useQuery } from "react-query";
import { format } from "date-fns";
import React from "react";
import { fetchNotifications } from "utils/query-keys";
import { getNotifications } from "services/warehouse-service";

function renderEvent({ macAddress, receivedAt }) {
  return `The ${macAddress} was last available at ${format(new Date(receivedAt), "HH:mm:ss")}`;
}

function UserNotifications({ searchTerm = "" }) {
  const [reload, updateReloadState] = useState(null);
  const forceUpdate = () => updateReloadState(Date.now());
  const [page, setPage] = useState(1);

  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchNotifications, page, searchTerm, reload], getNotifications);

  return (
    <Card className="h-100" style={{ width: "100%", height: "100%" }}>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={3}>
        <SuiBox display="flex" alignItems="center">
          <Icon>notifications</Icon>
          <SuiTypography variant="h6" fontWeight="medium">
            &nbsp;Alerts
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
              color={"error"}
              icon={"location_off"}
              title={
                <SuiTypography variant="caption" fontWeight="medium">
                  {renderEvent(item)}
                </SuiTypography>
              }
              dateTime={format(new Date(item.timeStamp), "HH:mm:ss dd/MM/y")}
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

export default UserNotifications;
