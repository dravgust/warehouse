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

const site = {
  name: " Conference Room",
  topLength: 4,
  leftLength: 4,
  error: -2,
  gateways: [
    {
      macAddress: "AC233FC011DF",
      name: "Circle Router",
      circumscribedRadius: 0,
      location: 4,
      envFactor: 1,
      gauge: {
        mac: "DD340206CB76",
        txPower: -39,
        rssi: 0,
        radius: 0,
        rssIs: [],
        originalRSSIs: [],
        isGage: false,
        location: 0,
      },
    },
  ],
  providerId: 1000,
  id: "62690483963d12edd88667c6",
};

const selectedSite = {
  site: {
    id: "62690483963d12edd88667c6",
    name: " Conference Room",
  },
  in: [
    {
      product: {
        id: "62d3cee39352a2547ba0c267",
        name: "White Tomato",
      },
      beacon: {
        macAddress: "DD3402061251",
        name: "white beacon 1",
      },
    },
    {
      product: {
        id: "62d3cee39352a2547ba0c267",
        name: "White Tomato",
      },
      beacon: {
        macAddress: "DD340206128B",
        name: "white beacon 2",
      },
    },
  ],
  out: [
    {
      product: {
        id: "62d3cee09352a2547ba0c266",
        name: "Black Cucumber",
      },
      beacon: {
        macAddress: "DD340206CB76",
        name: "black beacon 3",
      },
    },
    {
      product: {
        id: "62d3cee09352a2547ba0c266",
        name: "Black Cucumber",
      },
      beacon: {
        macAddress: "DD340206C5D9",
        name: "black beacon 1",
      },
    },
    {
      product: {
        id: "62d3cee09352a2547ba0c266",
        name: "Black Cucumber",
      },
      beacon: {
        macAddress: "DD340206C88E",
        name: "black beacon 2",
      },
    },
  ],
};

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
              {selectedSite.site.name}
            </SuiTypography>
          </SuiBox>
          <SuiBox>
            <SuiBox ref={cardRef} sx={{ width: "auto", height: "60vh" }}>
              {cardRef.current && (
                <CanvasSite width={width} height={height} site={site} selectedSite={selectedSite} />
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
