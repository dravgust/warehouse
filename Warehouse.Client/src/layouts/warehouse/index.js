import { useRef } from "react";
import Card from "@mui/material/Card";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import WarehouseCanvas from "./conponents/canvas-demo";
import { useStoreController } from "../../context/store.context";
import { useAuth } from "../../context/auth.context";

const Warehouse = () => {
  const [controller] = useStoreController();
  const { user } = useAuth();
  const cardRef = useRef();

  console.log("store", controller);
  console.log("user", user);
  console.log("width", cardRef.current.offsetWidth);

  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3} mb={3}>
        <Card>
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
            <SuiTypography variant="h6">Warehouse</SuiTypography>
          </SuiBox>
          <SuiBox ref={cardRef}>
            <WarehouseCanvas />
          </SuiBox>
        </Card>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default Warehouse;
