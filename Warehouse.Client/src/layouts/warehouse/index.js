import { useEffect, useRef, useState } from "react";
import { Card, Grid } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import CanvasSite from "./conponents/canvas-site";
import { useStoreController } from "../../context/store.context";
import { useAuth } from "../../context/auth.context";
import { useQuery } from "react-query";
import { fetchSiteById } from "../../utils/query-keys";
import { getSiteById } from "../../services/warehouse-service";
import CanvasList from "./conponents/canvas-list";

const Warehouse = () => {
  const [controller] = useStoreController();
  const { user } = useAuth();
  const cardRef = useRef();
  const [width, setWidth] = useState(0);
  const [height, setHeight] = useState(0);
  const { site: currentSite } = controller;

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
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchSiteById], () => getSiteById(currentSite.id), {
    enabled: Boolean(currentSite),
  });

  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3} mb={3}>
        <Card>
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
            <SuiTypography variant="h6" color={"info"}></SuiTypography>
          </SuiBox>
          <SuiBox px={2}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={12} lg={4} xl={4}>
                <CanvasList />
              </Grid>
              <Grid item xs={12} md={12} lg={8} xl={8}>
                <SuiBox ref={cardRef} sx={{ width: "auto", height: "60vh" }}>
                  {cardRef.current && isSuccess && (
                    <CanvasSite width={width} height={height} site={response} />
                  )}
                </SuiBox>
              </Grid>
            </Grid>
          </SuiBox>
        </Card>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default Warehouse;
