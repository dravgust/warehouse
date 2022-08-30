import React, { useState } from "react";
import { useQuery } from "react-query";
import Grid from "@mui/material/Grid";
import SuiBox from "components/SuiBox";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import ProductList from "./components/product-list";
import SelectedItem from "./components/selected-item";
import { Zoom } from "@mui/material";
import { fetchProductMetadata } from "../../utils/query-keys";
import { getProductMetadata } from "../../api/warehouse";

function Products() {
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

  const { data: metadata } = useQuery([fetchProductMetadata], getProductMetadata);

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
