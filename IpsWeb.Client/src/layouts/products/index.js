import React, { useState } from "react";
import { useQuery } from "react-query";
import { client } from "utils/api-client";
import * as auth from "auth-provider";

import Grid from "@mui/material/Grid";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";

// Soft UI Dashboard React examples
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";

import ProductList from "./components/product-list";
import SelectedItem from "./components/selected-item";
import { Zoom } from "@mui/material";

// Soft UI Dashboard React context
import { useSoftUIController } from "context";

function Products() {
  const [controller] = useSoftUIController();
  const { miniSidenav, sidenavColor } = controller;

  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [selectedItem, selectItem] = useState();

  const [refresh, updateRefreshState] = useState(0);
  const forceUpdate = () => updateRefreshState(Date.now());

  function handleDelete() {
    resetToNull();
    forceUpdate();
  }

  function handleSave() {
    forceUpdate();
  }

  const resetToNull = () => selectItem(null);
  const resetToDefault = () =>
    selectItem({
      id: "",
      name: "",
      description: "",
      macAddress: "",
      metadata,
    });

  const onSelectItem = (item) => {
    if (!item) return;
    let result = metadata
      ? metadata.map((e) => {
          let rm = item.metadata && item.metadata.find((m) => m.key === e.key);
          return rm && rm.value ? Object.assign({}, e, { value: rm.value }) : Object.assign({}, e);
        })
      : [];
    return selectItem({ ...item, metadata: result });
  };

  const fetchMetadata = async () => {
    const token = await auth.getToken();
    const res = await client(`items/metadata`, { token });
    return res.data;
  };
  const { data: metadata } = useQuery(["metadata"], fetchMetadata, {
    keepPreviousData: false,
    refetchOnWindowFocus: false,
  });

  const fetchRegisteredBeacons = async () => {
    const token = await auth.getToken();
    const res = await client(`sites/beacons-registered`, { token });
    return res;
  };
  const { data: beacons } = useQuery(["beacons-registered"], fetchRegisteredBeacons, {
    keepPreviousData: false,
    refetchOnWindowFocus: false,
  });

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox mb={3} py={3}>
        <Grid container spacing={3}>
          {/*<Grid item xs={12}>
            <Card sx={(theme) => card(theme, { miniSidenav })}>
              <CardContent sx={(theme) => cardContent(theme, { sidenavColor })}>
                <Grid container spacing={3} alignItems="center">
                  <Grid item sx={{ ml: "auto" }}>
                    <SuiBox
                      display="flex"
                      alignItems="center"
                      mt={{ xs: 2, sm: 0 }}
                      ml={{ xs: -1.5, sm: 0 }}
                    >
                      <SuiBox px={2}>
                        <SuiButton variant="gradient" color="white" onClick={resetToDefault}>
                          <Icon sx={{ fontWeight: "bold" }}>add</Icon>
                          &nbsp;new product
                        </SuiButton>
                      </SuiBox>
                    </SuiBox>
                  </Grid>
                </Grid>
              </CardContent>
            </Card>
          </Grid>*/}

          <Grid item xs={12} lg={selectedItem ? 5 : 12}>
            <ProductList
              searchTerm={searchTerm}
              selectedItem={selectedItem}
              selectItem={onSelectItem}
              resetToDefault={resetToDefault}
              refresh={refresh}
            />
          </Grid>
          <Zoom in={Boolean(selectedItem)}>
            <Grid item xs={12} lg={7}>
              {selectedItem && (
                <SelectedItem
                  item={selectedItem}
                  onSave={handleSave}
                  onDelete={handleDelete}
                  onClose={resetToNull}
                  beacons={beacons}
                />
              )}
            </Grid>
          </Zoom>
        </Grid>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
}

export default Products;
