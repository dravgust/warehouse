import Card from "@mui/material/Card";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import WarehouseCanvas from "./conponents/warehouse-canvas";
import { useStoreController } from "../../context/store.context";
import { useAuth } from "../../context/auth.context";

const Warehouse = () => {
  const [controller] = useStoreController();
  const { user } = useAuth();
  console.log("store", controller);
  console.log("user", user);
  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3}>
        <SuiBox mb={3}>
          <Card>
            <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
              <SuiTypography variant="h6">Warehouse</SuiTypography>
            </SuiBox>
            <SuiBox
              sx={{
                "& .MuiTableRow-root:not(:last-child)": {
                  "& td": {
                    borderBottom: ({ borders: { borderWidth, borderColor } }) =>
                      `${borderWidth[1]} solid ${borderColor}`,
                  },
                },
              }}
            >
              <WarehouseCanvas />
            </SuiBox>
          </Card>
        </SuiBox>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default Warehouse;
