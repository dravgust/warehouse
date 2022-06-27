import { useState } from "react";
import { Grid, Zoom } from "@mui/material";
import SuiBox from "components/SuiBox";
import Footer from "examples/Footer";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Gateways from "./components/gateways";
import Sites from "./components/sites";
import SetSite from "./components/set-site";

const SiteConfiguration = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [gateways, setGatweyList] = useState();
  function onSelectItem(item) {
    console.log("selected-site", item);
    setSelectedSite(item);
    setGatweyList(item.gateways);
  }

  const [siteForEdit, setSiteForEdit] = useState(null);
  const [selectedSite, setSelectedSite] = useState(null);

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox py={3}>
        <Grid container spacing={3}>

          {siteForEdit == null && (
                     <Grid item xs={12} md={4}>
                     <Sites onSelect={onSelectItem} onEdit={() => setSiteForEdit(selectedSite)}/>
                   </Grid>
          )}
          {siteForEdit && (
            <Grid item xs={12} md={4}>
              <SetSite item={siteForEdit} onClose={() => setSiteForEdit(null)} />
            </Grid>
          )}

          <Grid item xs={12} md={8}>
            <Gateways data={gateways} />
          </Grid>

        </Grid>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default SiteConfiguration;
