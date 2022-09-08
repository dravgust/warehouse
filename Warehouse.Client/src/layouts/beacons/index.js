import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import SuiBox from "components/SuiBox";
import { Zoom, Grid } from "@mui/material";
import { useState } from "react";
import BeaconList from "./components/beacon-list";
import SelectedBeacon from "./components/selected-beacon";
import { useQuery } from "react-query";
import { fetchBeaconMetadata } from "utils/query-keys";
import { getBeaconMetadata } from "api/warehouse";

const Beacons = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);

  const [selectedItem, selectItem] = useState(null);

  const [refresh, updateRefreshState] = useState(0);
  const forceUpdate = () => updateRefreshState(Date.now());

  const resetToNull = () => selectItem(null);
  const resetToDefault = () =>
    selectItem({
      macAddress: "",
      name: "",
    });
  const onSelectItem = (item, key) => {
    if (item) {
      let result = metadata
        ? metadata.map((e) => {
            let rm = item.metadata && item.metadata.find((m) => m.key === e.key);
            return rm && rm.value
              ? Object.assign({}, e, { value: rm.value })
              : Object.assign({}, e);
          })
        : [];
      selectItem({ ...item, metadata: result, key: key });
    }
  };

  function handleDelete() {
    resetToNull();
    forceUpdate();
  }

  const handleSave = () => {
    resetToNull();
    forceUpdate();
  };
  const { data: metadata } = useQuery([fetchBeaconMetadata], getBeaconMetadata);

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox>
        <SuiBox mb={3} py={3}>
          <Grid container spacing={2}>
            <Grid item xs={12} lg={selectedItem ? 5 : 12}>
              <BeaconList
                searchTerm={searchTerm}
                selectedItem={selectedItem}
                onRowSelect={onSelectItem}
                onAdd={resetToDefault}
                onDelete={handleDelete}
                refresh={refresh}
              />
            </Grid>
            <Zoom in={Boolean(selectedItem)}>
              <Grid item xs={12} lg={7}>
                {Boolean(selectedItem) && (
                  <SelectedBeacon item={selectedItem} onSave={handleSave} onClose={resetToNull} />
                )}
              </Grid>
            </Zoom>
          </Grid>
        </SuiBox>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default Beacons;
