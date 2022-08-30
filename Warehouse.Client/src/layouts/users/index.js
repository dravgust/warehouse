import SuiBox from "components/SuiBox";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import UserList from "./components/user-list";
import useSecurity, { SecurityPermissions } from "../../services/security-provider";
import React, { useState } from "react";
import { Grid, Zoom } from "@mui/material";
import UserEdit from "./components/user-edit";

function Users() {
  const { hasPermissions } = useSecurity("USER", SecurityPermissions.View);
  const [userEdit, setUserEdit] = useState(null);
  const resetToNull = () => setUserEdit(null);
  const resetPage = () => resetToNull();
  const onUserSave = () => resetPage();
  return (
    <DashboardLayout>
      <DashboardNavbar />
      {hasPermissions && (
        <SuiBox py={3}>
          <Grid container spacing={3}>
            <Zoom in={Boolean(userEdit)}>
              <Grid item xs={12}>
                {Boolean(userEdit) && (
                  <UserEdit item={userEdit} onClose={resetToNull} onSave={onUserSave} />
                )}
              </Grid>
            </Zoom>
            <Grid item xs={12}>
              <UserList onEdit={setUserEdit} />
            </Grid>
          </Grid>
        </SuiBox>
      )}
      <Footer />
    </DashboardLayout>
  );
}

export default Users;
