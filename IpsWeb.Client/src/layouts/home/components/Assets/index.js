import { useState } from "react";

// @mui material components
import Card from "@mui/material/Card";
import Icon from "@mui/material/Icon";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
/* eslint-disable react/prop-types */
import SuiAvatar from "components/SuiAvatar";

// Soft UI Dashboard Materail-UI example components
import Table from "examples/Tables/Table";

import { useQuery } from "react-query";
import { client } from "utils/api-client";
import * as auth from "auth-provider";
import { format, formatDistance } from "date-fns";

// Images
import beaconIcon from "assets/images/hotspot-tower.png";

function Assets({ searchTerm, selectedItem, onRowSelect = () => {} }) {
  const [page, setPage] = useState(1);
  const fetchItems = async (page, searchTerm) => {
    const token = await auth.getToken();
    const res = await client(`assets?page=${page}&size=3&searchTerm=${searchTerm}`, { token });
    return res;
  };
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery(["list-assets", page, searchTerm], () => fetchItems(page, searchTerm), {
    keepPreviousData: false,
    refetchOnWindowFocus: false,
  });

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
      <MenuItem onClick={closeMenu}>Action</MenuItem>
      <MenuItem onClick={closeMenu}>Another action</MenuItem>
      <MenuItem onClick={closeMenu}>Something else</MenuItem>
    </Menu>
  );

  function Beacon({ image, name, product }) {
    return (
      <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
        <SuiBox mr={2}>
          <SuiAvatar src={image} alt={name} size="sm" variant="rounded" />
        </SuiBox>
        <SuiBox display="flex" flexDirection="column">
          <SuiTypography variant="button" fontWeight="medium">
            {name}
          </SuiTypography>
          <SuiTypography variant="caption" color="secondary">
            {product ? product.name : "n/a"}
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    );
  }

  function Site({ timeStamp, site }) {
    return (
      <SuiBox display="flex" alignItems="center" px={1}>
        <SuiBox mr={2}>
          
        </SuiBox>
        <SuiBox display="flex" flexDirection="column">
          
          <SuiTypography variant="button" color="info" mt={-2}>
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
            Assets
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
              { name: "asset", align: "left" },
              { name: "last location", align: "center" },
            ]}
            rows={
              isSuccess &&
              response.data.map((item) => ({
                item: item,
                asset: <Beacon image={beaconIcon} name={item.macAddress} product={item.product} />,
                "last location": <Site timeStamp={item.timeStamp} site={item.site}></Site>,
              }))
            }
            page={page}
            totalPages={response.totalPages}
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
}

export default Assets;
