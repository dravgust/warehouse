import SuiBox from "components/SuiBox";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import UserList from "./components/user-list";
import useSecurity, { SecurityPermissions } from "services/security-provider";
import React, { useState } from "react";
import { Grid, Zoom } from "@mui/material";
import UserEdit from "./components/user-edit";

function Users() {
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
      username: "",
      password: "",
      phone: "",
      type: "Guest",
      providerId: 1000,
      logLevel: 0,
    });
  const onUserSave = () => resetPage();
  function handleDelete() {
    resetToNull();
    forceUpdate();
  }
  return (
    <DashboardLayout>
      <DashboardNavbar onSearch={onSearch} />
      {hasPermissions && (
        <SuiBox py={3}>
          <Grid container spacing={2}>
            <Zoom in={Boolean(userEdit)}>
              <Grid item xs={12}>
                {Boolean(userEdit) && (
                  <UserEdit item={userEdit} onClose={resetToNull} onSave={onUserSave} />
                )}
              </Grid>
            </Zoom>
            <Grid item xs={12}>
              <UserList
                searchTerm={searchTerm}
                onEdit={setUserEdit}
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
}

export default Users;
