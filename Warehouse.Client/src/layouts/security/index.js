import React from "react";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import SuiBox from "../../components/SuiBox";
import Footer from "../../examples/Footer";
import { Grid } from "@mui/material";
import useSecurity, { SecurityPermissions } from "../../services/security-provider";

const SecurityRoles = React.lazy(() => import("./components/roles"));
const SecurityObjects = React.lazy(() => import("./components/objects"));

const Security = () => {
  const { hasPermissions } = useSecurity("USER", SecurityPermissions.Grant);

  return (
    <DashboardLayout>
      <DashboardNavbar />
      {hasPermissions && (
        <SuiBox py={3}>
          <Grid container spacing={3}>
            <Grid item xs={12} lg={6}>
              <SecurityRoles />
            </Grid>
            <Grid item xs={12} lg={6}>
              <SecurityObjects />
            </Grid>
          </Grid>
        </SuiBox>
      )}
      <Footer />
    </DashboardLayout>
  );
};
export default Security;
