import { useState } from "react";
import Grid from "@mui/material/Grid";
import SuiBox from "components/SuiBox";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import ProductsTreeView from "./components/products";
import PositionEvents from "./components/position-events";
import Assets from "./components/beacons";
import { Stack, Zoom } from "@mui/material";
import SiteInfo from "./components/sites";
import BeaconTelemetry from "./components/beacon-telemetry";
import BeaconTelemetryCharts from "./components/beacon-charts/indiex";

function Dashboard() {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [selectedProduct, setSelectProduct] = useState(null);
  const [selectedSite, setSelectSite] = useState(null);
  const [selectedBeacon, setSelectBeacon] = useState(null);
  const [selectedList, setSelectList] = useState("product");

  const onListSelect = (listName) => {
    setSelectList(listName);
    setSelectProduct(null);
    setSelectBeacon(null);
  };

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox mb={3} py={3}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={12} lg={4} xl={4}>
            <Stack spacing={3}>
              {selectedList === "product" && (
                <ProductsTreeView
                  selectedProduct={selectedProduct}
                  onProductSelect={setSelectProduct}
                  selectedBeacon={selectedBeacon}
                  onBeaconSelect={setSelectBeacon}
                  onListSelect={onListSelect}
                />
              )}
              {selectedList === "beacon" && (
                <Assets
                  onListSelect={onListSelect}
                  onRowSelect={(row) =>
                    setSelectBeacon({ macAddress: row.macAddress, name: row.site.name })
                  }
                  searchTerm={searchTerm}
                  selectedItem={selectedBeacon}
                />
              )}
              {selectedList === "site" && (
                <SiteInfo
                  selectedSite={selectedSite}
                  onSiteSelect={setSelectSite}
                  selectedBeacon={selectedBeacon}
                  onBeaconSelect={setSelectBeacon}
                  onListSelect={onListSelect}
                />
              )}
            </Stack>
          </Grid>
          <Grid item xs={12} md={12} lg={8} xl={8}>
            <Grid container spacing={3}>
              {Boolean(selectedBeacon) && (
                <Zoom in={true}>
                  <Grid item xs={12} md={6}>
                    <BeaconTelemetry item={selectedBeacon} />
                  </Grid>
                </Zoom>
              )}
              <Grid item xs={12} md={6}>
                <PositionEvents searchTerm={selectedBeacon ? selectedBeacon.macAddress : ""} />
              </Grid>
              {Boolean(selectedBeacon) && (
                <Grid item xs={12} md={Boolean(selectedBeacon) ? 12 : 6}>
                  <BeaconTelemetryCharts item={selectedBeacon} />
                </Grid>
              )}
            </Grid>
          </Grid>
        </Grid>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
}

export default Dashboard;
