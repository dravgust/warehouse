import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import SuiBox from "../../components/SuiBox";
import Footer from "../../examples/Footer";
import SecurityRoles from "./components/roles";
import { Grid } from "@mui/material";
import SecurityObjects from "./components/objects";
import useSecurity, { SecurityPermissions } from "../../services/security-provider";

const Security = () => {
  const { hasPermissions } = useSecurity("USER", SecurityPermissions.Execute);

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
