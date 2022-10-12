import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import SuiBox from "../../components/SuiBox";
import { Grid, Zoom } from "@mui/material";
import Footer from "../../examples/Footer";
import React, { useState } from "react";
import useSecurity, { SecurityPermissions } from "../../services/security-provider";
import ProviderList from "./components/provider-list";
import ProviderEdit from "./components/provider-edit";

const Providers = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);
  const [reload, updateReloadState] = useState(0);
  const forceUpdate = () => updateReloadState(Date.now());
  const { hasPermissions: viewPermissions } = useSecurity("PROVIDER", SecurityPermissions.View);
  const [providerEdit, setProviderEdit] = useState(null);
  const resetToNull = () => setProviderEdit(null);
  const resetPage = () => {
    resetToNull();
    forceUpdate();
  };
  const resetToDefault = () =>
    setProviderEdit({
      id: 0,
      name: "",
      alias: "",
      description: "",
      culture: "en-US",
    });
  const onProviderSave = () => resetPage();
  function handleDelete() {
    resetToNull();
    forceUpdate();
  }
  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      {viewPermissions && (
        <SuiBox mb={3} py={1}>
          <Grid container spacing={2}>
            <Zoom in={Boolean(providerEdit)}>
              <Grid item xs={12}>
                {Boolean(providerEdit) && (
                  <ProviderEdit item={providerEdit} onClose={resetToNull} onSave={onProviderSave} />
                )}
              </Grid>
            </Zoom>
            <Grid item xs={12}>
              <ProviderList
                searchTerm={searchTerm}
                onEdit={setProviderEdit}
                onAdd={resetToDefault}
                onDelete={handleDelete}
                reload={reload}
              />
            </Grid>
          </Grid>
        </SuiBox>
      )}
      <Footer />
    </DashboardLayout>
  );
};
export default Providers;
