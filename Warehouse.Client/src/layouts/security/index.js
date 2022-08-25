import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import SuiBox from "../../components/SuiBox";
import Footer from "../../examples/Footer";
import SecurityRoles from "./components/roles";
import { Grid } from "@mui/material";
import SecurityObjects from "./components/objects";
import useSecurity from "../../services/security-provider";
import { useEffect } from "react";

const Security = () => {
  const { fetchRoles } = useSecurity();
  useEffect(() => {
    async function getRoles() {
      const roles = await fetchRoles();
      console.log(roles);
    }
    getRoles();
  }, []);

  return (
    <DashboardLayout>
      <DashboardNavbar />
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
      <Footer />
    </DashboardLayout>
  );
};
export default Security;
