import { useState } from "react";

// @mui material components
import Card from "@mui/material/Card";
import Icon from "@mui/material/Icon";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import Table from "examples/Tables/Table";
import { useQuery } from "react-query";
import { formatDistance } from "date-fns";
import beaconIcon from "assets/images/hotspot-tower.png";
import SensorsSharpIcon from "@mui/icons-material/SensorsSharp";
import { fetchAssets } from "../../../../utils/query-keys";
import { getAssets } from "../../../../services/warehouse-service";

function Assets({
  searchTerm = "",
  selectedItem,
  onRowSelect = () => {},
  onListSelect = () => {},
}) {
  const [page, setPage] = useState(1);
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchAssets, page, searchTerm], getAssets);

  const [menu, setMenu] = useState(null);

  const openMenu = ({ currentTarget }) => setMenu(currentTarget);
  const closeMenu = () => setMenu(null);

  const renderMenu = (
    <Menu
      id="simple-menu"
      anchorEl={menu}
      anchorOrigin={{
        vertical: "top",
        horizontal: "left",
      }}
      transformOrigin={{
        vertical: "top",
        horizontal: "right",
      }}
      open={Boolean(menu)}
      onClose={closeMenu}
    >
      <MenuItem
        onClick={() => {
          closeMenu();
          onListSelect("product");
        }}
      >
        Product List
      </MenuItem>
      {/*<MenuItem
        onClick={() => {
          closeMenu();
          onListSelect("site");
        }}
      >
        Site List
      </MenuItem>*/}
    </Menu>
  );

  function Beacon({ image, name, product }) {
    return (
      <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
        <SuiBox mr={2}>
          <SensorsSharpIcon fontSize="large" />
        </SuiBox>
        <SuiBox display="flex" flexDirection="column">
          <SuiTypography variant="button" fontWeight="medium">
            {name}
          </SuiTypography>
          <SuiTypography variant="caption" color="primary">
            {product ? product.name : "n/a"}
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    );
  }

  function Site({ timeStamp, site }) {
    return (
      <SuiBox display="flex" alignItems="center" px={1}>
        <SuiBox mr={2}></SuiBox>
        <SuiBox display="flex" flexDirection="column">
          <SuiTypography variant="h6" color="info" mt={-2}>
            {site ? site.name : "n/a"}
          </SuiTypography>
          <SuiTypography variant="caption" fontWeight="regular">
            {/*format(new Date(item.timeStamp), "dd/MM/yyy HH:mm:ss")*/}
            {formatDistance(new Date(timeStamp), new Date(), { addSuffix: true })}
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    );
  }

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Beacons
          </SuiTypography>
          <SuiBox display="flex" alignItems="center" lineHeight={0}>
            <Icon
              sx={{
                fontWeight: "bold",
                color: ({ palette: { info } }) => info.main,
                mt: -0.5,
              }}
            >
              done
            </Icon>
            <SuiTypography variant="button" fontWeight="regular" color="text">
              &nbsp;<strong>{response && response.totalItems}</strong> items
            </SuiTypography>
          </SuiBox>
        </SuiBox>
        <SuiBox color="text" px={2}>
          <Icon sx={{ cursor: "pointer", fontWeight: "bold" }} fontSize="small" onClick={openMenu}>
            more_vert
          </Icon>
        </SuiBox>
        {renderMenu}
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
              { name: "beacon", align: "left" },
              { name: "last location", align: "center" },
            ]}
            rows={
              isSuccess &&
              response.data.map((item) => ({
                key: item.macAddress,
                item: item,
                beacon: <Beacon image={beaconIcon} name={item.macAddress} product={item.product} />,
                "last location": <Site timeStamp={item.timeStamp} site={item.site}></Site>,
              }))
            }
            page={page}
            totalPages={response.totalPages}
            onPageChange={(event, value) => setPage(value)}
            onSelect={onRowSelect}
            selectedKey={selectedItem ? selectedItem.macAddress : ""}
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
}

export default Assets;
