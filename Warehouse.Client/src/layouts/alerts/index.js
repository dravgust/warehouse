import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import { useState } from "react";
import Footer from "../../examples/Footer";
import SuiBox from "../../components/SuiBox";
import { Grid, Zoom } from "@mui/material";
import SelectedAlert from "./components/selected-alert";
import AlertList from "./components/alert-list";

function WarehouseAlerts() {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);
  const [selectedItem, selectItem] = useState(null);
  const [refresh, updateRefreshState] = useState(0);
  const forceUpdate = () => updateRefreshState(Date.now());
  const resetToNull = () => selectItem(null);
  const resetToDefault = () =>
    selectItem({
      name: "",
      checkPeriod: 0,
      enabled: true,
    });
  const onSelectItem = (item, key) => {
    if (item) {
      selectItem({ ...item, key: key });
    }
  };

  function handleDelete() {
    resetToNull();
    forceUpdate();
  }
  const handleSave = () => forceUpdate();

  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      <SuiBox mb={3} py={3}>
        <Grid container spacing={3}>
          <Grid item xs={12} lg={selectedItem ? 5 : 12}>
            <AlertList
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
                <SelectedAlert
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
      <Footer />
    </DashboardLayout>
  );
}

export default WarehouseAlerts;
