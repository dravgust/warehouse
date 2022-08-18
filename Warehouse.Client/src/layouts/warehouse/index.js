import { useEffect, useRef, useState } from "react";
import Card from "@mui/material/Card";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import CanvasSite from "./conponents/canvas-site";
import { useStoreController } from "../../context/store.context";
import { useAuth } from "../../context/auth.context";

const Warehouse = () => {
  const [controller] = useStoreController();
  const { user } = useAuth();
  const cardRef = useRef();
  const [width, setWidth] = useState(0);
  const [height, setHeight] = useState(0);

  const resize = () => {
    if (!cardRef.current) return;
    setWidth(cardRef.current.offsetWidth);
    setHeight(cardRef.current.offsetHeight);
  };
  useEffect(() => {
    window.addEventListener("resize", resize);
    cardRef.current && resize();
    return () => {
      window.removeEventListener("resize", resize);
    };
  }, [cardRef]);

  console.log("store", controller);
  console.log("user", user);

  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3} mb={3}>
        <Card>
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
            <SuiTypography variant="h6" color={"info"}>
              {controller.site ? controller.site.site.name : ""}
            </SuiTypography>
          </SuiBox>
          <SuiBox>
            <SuiBox ref={cardRef} sx={{ width: "auto", height: "60vh" }}>
              {controller.site && cardRef.current && (
                <CanvasSite width={width} height={height} selectedSite={controller.site} />
              )}
            </SuiBox>
          </SuiBox>
        </Card>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default Warehouse;
