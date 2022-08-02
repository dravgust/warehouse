import { Card, Icon } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiButton from "components/SuiButton";
import Table from "examples/Tables/Table";
import { useState } from "react";
import { useQuery } from "react-query";
import SuiAvatar from "components/SuiAvatar";
import alertIcon from "assets/images/notification-alarm-buzzer-icon.png";
import { fetchAlerts } from "utils/query-keys";
import { getAlerts } from "services/warehouse-service";

const AlertList = ({
  searchTerm,
  selectedItem,
  onRowSelect = (item) => {},
  onAdd = () => {},
  refresh,
}) => {
  const [page, setPage] = useState(1);
  const { isLoading, error, data, isSuccess } = useQuery(
    [fetchAlerts, page, searchTerm, refresh],
    getAlerts
  );

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Alerts
          </SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
          <SuiButton variant="gradient" color="primary" onClick={onAdd}>
            <Icon sx={{ fontWeight: "bold" }}>add</Icon>
            &nbsp;new
          </SuiButton>
        </SuiBox>
      </SuiBox>

      <SuiBox
        sx={{
          "& .MuiTableRow-root:not(:last-child)": {
            "& td": {
              borderBottom: ({ borders: { borderWidth, borderColor } }) =>
                `${borderWidth[1]} solid ${borderColor}`,
            },
          },
        }}
      >
        {isSuccess && (
          <Table
            columns={[
              { name: "name", align: "left" },
              { name: "check period", align: "left" },
              { name: "email", align: "center" },
              { name: "sms", align: "center" },
              { name: "enabled", align: "center" },
            ]}
            rows={data.items.map((item) => ({
              selectedItem: selectedItem,
              item: item,
              name: (
                <SuiBox display="flex" alignItems="center">
                  <SuiBox mx={2}>
                    <SuiAvatar src={alertIcon} alt={item.id} size="sm" variant="rounded" />
                  </SuiBox>
                  <SuiTypography variant="button" fontWeight="medium">
                    {item.name ? item.name : "n/a"}
                  </SuiTypography>
                </SuiBox>
              ),
              "check period": (
                <SuiTypography variant="caption" color="text" fontWeight="medium" px={2}>
                  {item.checkPeriod} sec
                </SuiTypography>
              ),
              email: (
                <SuiTypography variant="caption" color={"error"} fontWeight="medium">
                  {"No"}
                </SuiTypography>
              ),
              sms: (
                <SuiTypography variant="caption" color={"error"} fontWeight="medium">
                  {"No"}
                </SuiTypography>
              ),
              enabled: (
                <SuiTypography
                  variant="caption"
                  color={item.enabled ? "success" : "error"}
                  fontWeight="medium"
                >
                  {item.enabled ? "Yes" : "No"}
                </SuiTypography>
              ),
            }))}
            page={page}
            totalPages={data.totalPages}
            onPageChange={(event, value) => setPage(value)}
            onSelect={onRowSelect}
            selectedKey={selectedItem ? selectedItem.key : ""}
          />
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
};
export default AlertList;
