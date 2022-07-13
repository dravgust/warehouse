import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import Footer from "../../examples/Footer";
import SuiBox from "../../components/SuiBox";
import Grid from "@mui/material/Grid";
import { Zoom } from "@mui/material";
import { useState } from "react";
import BeaconList from "./components/beacon-list";
import SelectedBeacon from "./components/selected-beacon";
import * as auth from "../../auth-provider";
import { client } from "../../utils/api-client";
import { useQuery } from "react-query";

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
    if (!item) return;
    let result = metadata
      ? metadata.map((e) => {
          let rm = item.metadata && item.metadata.find((m) => m.key === e.key);
          return rm && rm.value ? Object.assign({}, e, { value: rm.value }) : Object.assign({}, e);
        })
      : [];
    return selectItem({ ...item, metadata: result, key: key });
  };

  function handleDelete() {
    resetToNull();
    forceUpdate();
  }

  const handleSave = () => forceUpdate();

  const fetchMetadata = async () => {
    const token = await auth.getToken();
    const res = await client(`items/item-metadata`, { token });
    return res.data;
  };
  const { data: metadata } = useQuery(["item-metadata"], fetchMetadata, {
    keepPreviousData: false,
    refetchOnWindowFocus: false,
  });

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox>
        <SuiBox mb={3} py={3}>
          <Grid container spacing={3}>
            <Grid item xs={12} lg={selectedItem ? 5 : 12}>
              <BeaconList
                searchTerm={searchTerm}
                selectedItem={selectedItem}
                onRowSelect={onSelectItem}
                onAdd={resetToDefault}
                refresh={refresh}
              />
            </Grid>
            <Zoom in={Boolean(selectedItem)}>
              <Grid item xs={12} lg={7}>
                {Boolean(selectedItem) && (
                  <SelectedBeacon
                    item={selectedItem}
                    onSave={handleSave}
                    onDelete={handleDelete}
                    onClose={resetToNull}
                  />
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
