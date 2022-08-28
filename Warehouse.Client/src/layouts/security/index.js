import React, { Suspense } from "react";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import SuiBox from "../../components/SuiBox";
import Footer from "../../examples/Footer";
import { Card, Grid, Stack } from "@mui/material";
import useSecurity, { SecurityPermissions } from "../../services/security-provider";
import SecurityRoles from "./components/roles";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import { useState } from "react";
import SecurityIcon from "@mui/icons-material/Security";
import GppGoodIcon from "@mui/icons-material/GppGood";

const SecurityObjects = React.lazy(() => import("./components/objects"));

const Security = () => {
  const { hasPermissions } = useSecurity("USER", SecurityPermissions.Grant);
  const [selectedView, setSelectView] = useState(0);
  const handleChange = (event, value) => {
    setSelectView(value);
  };
  return (
    <DashboardLayout>
      <DashboardNavbar />
      {hasPermissions && (
        <SuiBox py={3}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Card>
                <SuiBox p={3}>
                  <Tabs value={selectedView} onChange={handleChange} aria-label="navigation">
                    <Tab icon={<SecurityIcon />} iconPosition="start" label="Roles" />
                    <Tab icon={<GppGoodIcon />} iconPosition="start" label="Security Objects" />
                  </Tabs>
                </SuiBox>
              </Card>
            </Grid>
            {selectedView === 0 ? (
              <Grid item xs={12} lg={12}>
                <SecurityRoles />
              </Grid>
            ) : (
              <Grid item xs={12} lg={12}>
                <Suspense fallback={<>Loading...</>}>
                  <SecurityObjects />
                </Suspense>
              </Grid>
            )}
          </Grid>
        </SuiBox>
      )}
      <Footer />
    </DashboardLayout>
  );
};
export default Security;
