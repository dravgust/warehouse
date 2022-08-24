import { useEffect, useRef, useState } from "react";
import { Card, Grid, Icon, IconButton, Tooltip } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import CanvasSite from "./conponents/canvas-site";
import { setProduct, setSite, useStoreController, setBeacon } from "../../context/store.context";
import { useAuth } from "../../context/auth.context";
import { useQuery } from "react-query";
import { fetchSiteById } from "../../utils/query-keys";
import { getSiteById } from "../../services/warehouse-service";
import CanvasList from "./conponents/canvas-list/index0";
import { useNavigate } from "react-router-dom";
import CanvasListByProduct from "./conponents/canvas-list/list-by-product";

const Warehouse = () => {
  const [controller, dispatch] = useStoreController();
  const { user } = useAuth();
  const cardRef = useRef();
  const [width, setWidth] = useState(0);
  const [height, setHeight] = useState(0);

  const { site: currentSite, product: currentProduct, beacon: selectedBeacon } = controller;
  const onBeaconSelect = (item) => setBeacon(dispatch, item);
  const onProductSelect = (item) => {
    onBeaconSelect(null);
    setProduct(dispatch, item);
  };
  const onSiteSelect = (item) => {
    onBeaconSelect(null);
    setSite(dispatch, item);
  };

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
  const navigate = useNavigate();
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchSiteById, currentSite], () => getSiteById(currentSite.id), {
    enabled: Boolean(currentSite),
  });

  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3} mb={3}>
        <Card>
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
            <SuiTypography
              variant="h6"
              color={
                currentSite && currentSite.products
                  ? "info"
                  : currentProduct && currentProduct.sites
                  ? "primary"
                  : "secondary"
              }
            >
              {currentSite && currentSite.products
                ? currentSite.name
                : currentProduct && currentProduct.sites
                ? currentProduct.name
                : "n/a"}
            </SuiTypography>
            <SuiBox display="flex" alignItems="center" mt={{ xs: -1, sm: 0 }}>
              <IconButton size="xl" color="inherit" onClick={() => navigate("/home")}>
                <Tooltip title="Close">
                  <Icon>close</Icon>
                </Tooltip>
              </IconButton>
            </SuiBox>
          </SuiBox>
          <SuiBox px={2}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={12} lg={4} xl={4}>
                {currentSite && currentSite.products && (
                  <CanvasList
                    selectedSite={currentSite}
                    selectedProduct={currentProduct}
                    onProductSelect={onProductSelect}
                    selectedBeacon={selectedBeacon}
                    onBeaconSelect={onBeaconSelect}
                  />
                )}
                {currentProduct && currentProduct.sites && (
                  <CanvasListByProduct
                    selectedSite={currentSite}
                    selectedProduct={currentProduct}
                    onSiteSelect={onSiteSelect}
                    selectedBeacon={selectedBeacon}
                    onBeaconSelect={onBeaconSelect}
                  />
                )}
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
