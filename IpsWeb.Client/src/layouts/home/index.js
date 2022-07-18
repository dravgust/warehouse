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
import Sites from "./components/sites";
import BeaconTelemetry from "./components/beacon-telemetry";
import GradientLineChart from "../../examples/Charts/LineCharts/GradientLineChart";
import Icon from "@mui/material/Icon";
import SuiTypography from "../../components/SuiTypography";
import gradientLineChartData from "./data/gradientLineChartData";
import typography from "../../assets/theme/base/typography";

function Dashboard() {
  const { size } = typography;
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [selectedProduct, setSelectProduct] = useState(null);
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
              {selectedList === "site" && <Sites />}
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
              <Grid item xs={12} md={Boolean(selectedBeacon) ? 12 : 6}>
                <GradientLineChart
                  title="Temperature Overview"
                  description={
                    <SuiBox display="flex" alignItems="center">
                      <SuiBox fontSize={size.lg} color="success" mb={0.3} mr={0.5} lineHeight={0}>
                        <Icon className="font-bold">arrow_upward</Icon>
                      </SuiBox>
                      <SuiTypography variant="button" color="text" fontWeight="medium">
                        -% more{" "}
                        <SuiTypography variant="button" color="text" fontWeight="regular">
                          in ----
                        </SuiTypography>
                      </SuiTypography>
                    </SuiBox>
                  }
                  height="20.25rem"
                  chart={gradientLineChartData}
                />
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
}

export default Dashboard;
