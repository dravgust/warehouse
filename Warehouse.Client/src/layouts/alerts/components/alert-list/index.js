import { ButtonGroup, Card, Icon } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiButton from "components/SuiButton";
import Table from "examples/Tables/Table";
import { useState } from "react";
import { useQuery } from "react-query";
import SuiAvatar from "components/SuiAvatar";
import alertIcon from "assets/images/notification-alarm-buzzer-icon.png";
import { fetchAlerts } from "utils/query-keys";
import { deleteAlert, getAlerts } from "api/warehouse";
import DeletePrompt from "components/DeletePrompt";

const AlertList = ({
  searchTerm,
  selectedItem,
  onRowSelect = () => {},
  onAdd = () => {},
  onDelete = () => {},
  refresh,
}) => {
  const [page, setPage] = useState(1);
  const { isLoading, error, data, isSuccess } = useQuery(
    [fetchAlerts, page, searchTerm, refresh],
    getAlerts
  );

  const handleDelete = async (item) => {
    try {
      await deleteAlert(item);
      return onDelete();
    } catch (err) {
      console.log("delete-item", err);
    }
  };

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
              { name: "check period", align: "center" },
              { name: "email", align: "center" },
              { name: "sms", align: "center" },
              { name: "enabled", align: "center" },
              { name: "", align: "center" },
            ]}
            rows={data.items.map((item, index) => ({
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
              "": (
                <ButtonGroup variant="text" aria-label="text button group" color="text">
                  <SuiButton
                    variant="text"
                    color="dark"
                    onClick={() => onRowSelect(item, `row-${index}`)}
                  >
                    <Icon>border_color</Icon>
                  </SuiButton>
                  <DeletePrompt
                    renderButton={(handleClickOpen) => (
                      <SuiButton
                        variant="text"
                        color="error"
                        onClick={(e) => {
                          e.stopPropagation();
                          handleClickOpen();
                          e.preventDefault();
                        }}
                      >
                        <Icon>delete</Icon>
                      </SuiButton>
                    )}
                    onDelete={() => handleDelete(item)}
                  />
                </ButtonGroup>
              ),
            }))}
            page={page}
            totalPages={data.totalPages}
            onPageChange={(event, value) => setPage(value)}
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
