import { useState } from "react";

// @mui material components
import Grid from "@mui/material/Grid";
import Icon from "@mui/material/Icon";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";

// Soft UI Dashboard React examples
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";


import ProductsTreeView from "./components/products";
import PositionEvents from "./components/position-events";
import Assets from "./components/beacons";

function Dashboard() {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [selectedProduct, setSelectProduct] = useState(null);
  const [selectedBeacon, setSelectBeacon] = useState('');

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox mb={3} py={3}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={6} lg={4}>
            <SuiBox mb={3}>
              <ProductsTreeView
                  selectedProduct={selectedProduct}
                  onProductSelect={setSelectProduct}
                  selectedBeacon={selectedBeacon}
                  onBeaconSelect={setSelectBeacon}
              />
            </SuiBox>
          </Grid>
          <Grid item xs={12} md={6} lg={4}>
            <Assets />
          </Grid>
          <Grid item xs={12} md={6} lg={4}>
            <PositionEvents searchTerm={selectedBeacon}/>
          </Grid>
        </Grid>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
}

export default Dashboard;
