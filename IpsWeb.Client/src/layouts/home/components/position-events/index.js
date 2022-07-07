import { useState } from "react";

// @mui material components
import Card from "@mui/material/Card";
import Icon from "@mui/material/Icon";
import { IconButton, Tooltip } from "@mui/material";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

// Soft UI Dashboard React examples
import TimelineItem from "examples/Timeline/TimelineItem";

import { useQuery } from "react-query";
import { client } from "utils/api-client";
import * as auth from "auth-provider";
import { format, formatDistance } from "date-fns";

function PositionEvents({ searchTerm }) {
  const [reload, updateReloadState] = useState();
  const forceUpdate = () => updateReloadState(Date.now());

  const fetchItems = async (searchTerm) => {
    const token = await auth.getToken();
    const res = await client(`events?page=${1}&size=12&searchTerm=${searchTerm}`, { token });
    return res;
  };
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery(["list-events", reload, searchTerm], () => fetchItems(searchTerm), {
    keepPreviousData: false,
    refetchOnWindowFocus: false,
  });

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
              icon={item.event == "IN" ? "add_business" : "store"}
              title={
                <SuiTypography variant="caption" fontWeight="medium">
                  The device  {item.macAddress} is {item.event === "OUT" ? "out of the" : "entered"} the <span style={{ color: "#344767" }}>{item.siteName}</span>
                </SuiTypography>
              }
              dateTime={
                formatDistance(new Date(item.timeStamp), new Date(), { addSuffix: true })
              }
            />
          ))}
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
