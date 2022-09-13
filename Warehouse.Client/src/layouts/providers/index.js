import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import SuiBox from "../../components/SuiBox";
import { Grid, Zoom } from "@mui/material";
import Footer from "../../examples/Footer";
import React, { useState } from "react";
import useSecurity, { SecurityPermissions } from "../../services/security-provider";
import ProviderList from "./components/provider-list";

const Providers = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const onSearch = (value) => setSearchTerm(value);
  const [reload, updateReloadState] = useState(0);
  const forceUpdate = () => updateReloadState(Date.now());
  const { hasPermissions } = useSecurity("USER", SecurityPermissions.View);
  const [userEdit, setUserEdit] = useState(null);
  const resetToNull = () => setUserEdit(null);
  const resetPage = () => {
    resetToNull();
    forceUpdate();
  };
  const resetToDefault = () =>
    setUserEdit({
      id: 0,
      name: "",
    });
  const onUserSave = () => resetPage();
  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      {hasPermissions && (
        <SuiBox py={3}>
          <Grid container spacing={2}>
            <Zoom in={Boolean(userEdit)}>
              <Grid item xs={12}>
                {Boolean(userEdit) && <>...</>}
              </Grid>
            </Zoom>
            <Grid item xs={12}>
              <ProviderList
                searchTerm={searchTerm}
                onEdit={setUserEdit}
                onAdd={resetToDefault}
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
