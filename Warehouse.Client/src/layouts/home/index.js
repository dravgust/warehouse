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
import React from "react";
import { Card } from "@mui/material";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import QrCode2SharpIcon from "@mui/icons-material/QrCode2Sharp";
import SensorsOutlinedIcon from "@mui/icons-material/SensorsOutlined";
import TabOutlinedIcon from "@mui/icons-material/TabOutlined";
import UserNotifications from "./components/notifications";

function Dashboard() {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [selectedProduct, setSelectProduct] = useState(null);
  const [selectedSite, setSelectSite] = useState(null);
  const [selectedBeacon, setSelectBeacon] = useState(null);
  const [selectedView, setSelectView] = useState(0);

  const handleChange = (event, value) => {
    setSelectProduct(null);
    setSelectBeacon(null);
    setSelectView(value);
  };

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox mb={3} py={3}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={12} lg={4} xl={4}>
            <Stack spacing={3}>
              <Card>
                <SuiBox p={3}>
                  <Tabs value={selectedView} onChange={handleChange} aria-label="navigation">
                    <Tab icon={<TabOutlinedIcon />} iconPosition="start" label="Sites" />
                    <Tab icon={<QrCode2SharpIcon />} iconPosition="start" label="Products" />
                    <Tab icon={<SensorsOutlinedIcon />} iconPosition="start" label="Beacons" />
                  </Tabs>
                </SuiBox>
              </Card>
              {selectedView === 1 && (
                <ProductsTreeView
                  selectedProduct={selectedProduct}
                  onProductSelect={setSelectProduct}
                  selectedBeacon={selectedBeacon}
                  onBeaconSelect={setSelectBeacon}
                />
              )}
              {selectedView === 2 && (
                <Assets
                  onRowSelect={(row) =>
                    setSelectBeacon({
                      macAddress: row.macAddress,
                      name: row.site ? row.site.name : null,
                    })
                  }
                  searchTerm={searchTerm}
                  selectedItem={selectedBeacon}
                />
              )}
              {selectedView === 0 && (
                <SiteInfo
                  selectedSite={selectedSite}
                  onSiteSelect={setSelectSite}
                  selectedBeacon={selectedBeacon}
                  onBeaconSelect={setSelectBeacon}
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
              <Grid item xs={12} md={Boolean(selectedBeacon) ? 6 : 12}>
                <Stack
                  spacing={3}
                  direction={Boolean(selectedBeacon) ? "column" : "row"}
                  style={{ height: "100% " }}
                >
                  <PositionEvents searchTerm={selectedBeacon ? selectedBeacon.macAddress : ""} />
                  <UserNotifications searchTerm={selectedBeacon ? selectedBeacon.macAddress : ""} />
                </Stack>
              </Grid>
              {Boolean(selectedBeacon) && (
                <Grid item xs={12}>
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
