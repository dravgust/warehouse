import { ButtonGroup, Card, Icon } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiButton from "components/SuiButton";
import Table from "examples/Tables/Table";
import { useState } from "react";
import { useQuery } from "react-query";
import SuiAvatar from "components/SuiAvatar";
import beaconIcon from "assets/images/hotspot-tower.png";
import { fetchBeacons } from "utils/query-keys";
import { deleteBeacon, getBeacons } from "api/warehouse";
import DeletePrompt from "../../../site-configuration/components/delete-promt";

const BeaconList = ({
  searchTerm,
  selectedItem,
  onRowSelect = (item) => {},
  onAdd = () => {},
  onDelete = () => {},
  refresh,
}) => {
  const [page, setPage] = useState(1);
  const { isLoading, error, data, isSuccess } = useQuery(
    [fetchBeacons, page, searchTerm, refresh],
    getBeacons
  );

  const handleDelete = async (item) => {
    try {
      await deleteBeacon(item);
      return onDelete();
    } catch (err) {
      console.log("delete-beacon", err);
    }
  };

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Beacons
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
              { name: "mac", align: "left" },
              { name: "name", align: "center" },
              { name: "product name", align: "center" },
              { name: "", align: "center" },
            ]}
            rows={data.items.map((item, index) => ({
              selectedItem: selectedItem,
              item: item,
              mac: (
                <SuiBox display="flex" alignItems="center">
                  <SuiBox mx={2}>
                    <SuiAvatar src={beaconIcon} alt={item.macAddress} size="sm" variant="rounded" />
                  </SuiBox>
                  <SuiTypography variant="button" fontWeight="medium">
                    {item.macAddress}
                  </SuiTypography>
                </SuiBox>
              ),
              name: (
                <SuiTypography variant="caption" color="text" fontWeight="medium" px={2}>
                  {item.name ? item.name : ""}
                </SuiTypography>
              ),
              "product name": (
                <SuiTypography variant="caption" color="text" fontWeight="medium">
                  {item.product ? item.product.name : "n/a"}
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
export default BeaconList;
