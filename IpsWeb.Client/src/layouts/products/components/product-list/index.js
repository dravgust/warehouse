import React, { useState, useEffect } from "react";
import { useQuery } from "react-query";
import { client } from "utils/api-client";
import * as auth from "auth-provider";

// @mui material components
import SuiBox from "components/SuiBox";
import { Card, Icon, IconButton, Tooltip } from "@mui/material";
import SuiTypography from "components/SuiTypography";

import SuiButton from "components/SuiButton";

// Soft UI Dashboard Materail-UI example components
import Table from "examples/Tables/Table";
import SetProduct from "./set-product";
import ProductItem from "../product-item";

import Pagination from "@mui/material/Pagination";
import Stack from "@mui/material/Stack";

function ProductList({
  searchTerm,
  selectedItem,
  selectItem = (item) => {},
  resetToDefault = () => {},
  refresh,
}) {
  const [reload, updateReloadState] = useState(null);
  const forceUpdate = () => updateReloadState(Date.now());

  const [page, setPage] = useState(1);
  const fetchItems = async (searchTerm, page) => {
    const token = await auth.getToken();
    const res = await client(`items?searchTerm=${searchTerm}&page=${page}&size=6`, { token });
    return res;
  };
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery(
    ["list-items", page, searchTerm, refresh, reload],
    () => fetchItems(searchTerm, page),
    {
      keepPreviousData: false,
      refetchOnWindowFocus: false,
    }
  );

  useEffect(() => {
    if (isSuccess && response.data.length > 0) {
      if (selectedItem) {
        var e = response.data.find((item) => selectedItem.id === item.id);
        if (e) {
          selectItem(e);
        } else if (selectedItem.id) {
          selectItem(null);
        }
      }
    }
  }, [isSuccess]);

  const [open, setOpen] = React.useState(false);
  const [currentItem, setCurrentItem] = React.useState();

  const handleAddClickOpen = () => {
    setOpen(true);
  };

  const handleEditClickOpen = (item) => {
    setCurrentItem(item);
    setOpen(true);
  };

  const handleClose = () => {
    setCurrentItem(null);
    setOpen(false);
  };

  return (
    <Card id="product-list">
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={3}>
        <SuiBox pt={0} px={2}>
          <SuiTypography variant="h6" fontWeight="medium">
            Products
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

        <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
          <SuiButton variant="gradient" color="primary" onClick={resetToDefault}>
            <Icon sx={{ fontWeight: "bold" }}>add</Icon>
            &nbsp;new
          </SuiButton>

          <IconButton size="xl" color="inherit" onClick={forceUpdate}>
            <Tooltip title="Reload">
              <Icon>sync</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox pt={1} pb={2} px={2}>
        <SuiBox component="ul" display="flex" flexDirection="column">
          {isSuccess &&
            response.data.map((item) => (
              <ProductItem
                isSelected={selectedItem && selectedItem.id === item.id}
                key={item.id}
                item={item}
                onClick={() => {
                  selectItem(item);
                }}
              />
            ))}
          {isLoading && <SuiTypography color="secondary">Loading..</SuiTypography>}
          {error && <SuiTypography color="error">Error occurred!</SuiTypography>}
        </SuiBox>
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
      </SuiBox>
    </Card>
  );

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Products
          </SuiTypography>
          <SuiBox display="flex" alignItems="center" lineHeight={0}>
            {/*<Icon
              sx={{
                fontWeight: "bold",
                color: ({ palette: { info } }) => info.main,
                mt: -0.5,
              }}
            >
              done
            </Icon>
            <SuiTypography variant="button" fontWeight="regular" color="text">
              &nbsp;<strong>30 done</strong> this month
            </SuiTypography>*/}
          </SuiBox>
        </SuiBox>
        <SuiBox px={2}>
          <SuiButton variant="gradient" color="dark" onClick={handleAddClickOpen}>
            <Icon sx={{ fontWeight: "bold" }}>add</Icon>
            &nbsp;add new product
          </SuiButton>
          <SetProduct open={open} handleClose={handleClose} editItem={currentItem} />
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
              { name: "description", align: "left" },
            ]}
            rows={response.data.map((item) => ({
              name: (
                <SuiTypography variant="caption" color="text" fontWeight="medium" px={2}>
                  {item.name}
                </SuiTypography>
              ),
              description: (
                <SuiTypography variant="caption" color="text" fontWeight="medium">
                  {item.description}
                </SuiTypography>
              ),
            }))}
            page={page}
            totalPages={response.totalPages}
            onPageChange={(event, value) => setPage(value)}
          />
        )}
        {isLoading && <SuiTypography px={2} color="secondary">Loading..</SuiTypography>}
        {error && <SuiTypography px={2} color="err">Error occurred!</SuiTypography>}
      </SuiBox>
    </Card>
  );
}

export default ProductList;
