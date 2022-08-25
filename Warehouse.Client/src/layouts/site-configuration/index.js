import { useState } from "react";
import { Grid, Zoom } from "@mui/material";
import SuiBox from "components/SuiBox";
import Footer from "examples/Footer";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Gateways from "./components/gateways";
import Sites from "./components/sites";
import SetSite from "./components/set-site";
import SetGateway from "./components/set-gateway";
import { useQuery } from "react-query";
import { fetchRegisteredBeacons, fetchRegisteredGw } from "../../utils/query-keys";
import { getRegisteredBeacons, getRegisteredGw } from "../../api/warehouse";

const SiteConfiguration = () => {
  const [refresh, updateRefreshState] = useState(null);
  const forceUpdate = () => updateRefreshState(Date.now());

  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [siteForEdit, setSiteForEdit] = useState(null);
  const [selectedSite, setSelectedSite] = useState(null);
  const onSelectItem = (item, key) => {
    resetGwToNull();
    if (siteForEdit) {
      setSiteForEdit({ ...item, siteId: item.id });
    }
    setSelectedSite({ ...item, key: key });
  };
  const resetToNull = () => setSiteForEdit(null);
  const resetToDefault = () =>
    setSiteForEdit({
      id: "",
      name: "",
      topLength: 0,
      leftLength: 0,
      error: 0,
    });

  const [gwForEdit, setGwForEdit] = useState(null);
  const resetGwToNull = () => setGwForEdit(null);
  const resetGwToDefault = () =>
    setGwForEdit({
      siteId: selectedSite.id,
      macAddress: "",
      name: "",
      circumscribedRadius: 0,
      location: 0,
      envFactor: 0,
    });
  const onSiteSave = () => resetPage();
  const resetPage = () => {
    resetGwToNull();
    resetToNull();
    setSelectedSite(null);
    forceUpdate();
  };
  const onSiteDelete = () => resetPage();
  const onGwSave = () => resetPage();
  const onGwDelete = () => resetPage();

  const { data: beacons } = useQuery([fetchRegisteredBeacons], getRegisteredBeacons);
  const { data: gateways } = useQuery([fetchRegisteredGw], getRegisteredGw);

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox mb={3} py={3}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={5}>
            <Grid container spacing={siteForEdit ? 3 : 0}>
              <Zoom in={Boolean(siteForEdit)}>
                <Grid item xs={12}>
                  {Boolean(siteForEdit) && (
                    <SetSite item={siteForEdit} onClose={resetToNull} onSave={onSiteSave} />
                  )}
                </Grid>
              </Zoom>
              <Grid item xs={12}>
                <Sites
                  onSelect={onSelectItem}
                  selectedItem={selectedSite}
                  onEdit={() => {
                    setSiteForEdit(selectedSite);
                  }}
                  onAdd={resetToDefault}
                  onDelete={onSiteDelete}
                  refresh={refresh}
                  searchTerm={searchTerm}
                />
              </Grid>
            </Grid>
          </Grid>
          <Grid item xs={12} md={7}>
            <Grid container spacing={gwForEdit ? 3 : 0}>
              <Zoom in={Boolean(gwForEdit)}>
                <Grid item xs={12}>
                  {Boolean(gwForEdit) && (
                    <SetGateway
                      item={gwForEdit}
                      onClose={resetGwToNull}
                      onSave={onGwSave}
                      gateways={gateways}
                      beacons={beacons}
                    />
                  )}
                </Grid>
              </Zoom>
              <Zoom in={true}>
                <Grid item xs={12}>
                  <Gateways
                    data={selectedSite}
                    onEdit={setGwForEdit}
                    onAdd={resetGwToDefault}
                    onDelete={onGwDelete}
                  />
                </Grid>
              </Zoom>
            </Grid>
          </Grid>
        </Grid>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default SiteConfiguration;
