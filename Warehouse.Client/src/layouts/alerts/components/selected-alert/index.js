import React from "react";
// prop-types is a library for typechecking of props
import PropTypes from "prop-types";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { Card, Icon, IconButton, Tooltip } from "@mui/material";
import AlertForm from "./alert-form";

function SelectedAlert({ onSave = () => {}, onClose = () => {}, item = {} }) {
  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={3}>
        <SuiBox pt={0} px={2}>
          <SuiTypography variant="h6" fontWeight="medium">
            Alert
          </SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: -1, sm: 0 }}>
          <IconButton size="xl" color="inherit" onClick={onClose}>
            <Tooltip title="Close">
              <Icon>close</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox
        component="div"
        display="flex"
        justifyContent="space-between"
        alignItems="flex-start"
        bgColor="rgba(203, 12, 159, 0.08)"
        borderRadius="lg"
        px={2}
        py={2}
        mb={2}
        mt={3}
        mx={2}
        style={{ border: "1px solid rgba(0, 0, 0, 0.125)" }}
      >
        <SuiBox width="100%" display="flex" flexDirection="column">
          <SuiBox
            display="flex"
            justifyContent="space-between"
            alignItems={{ xs: "flex-start", sm: "center" }}
            flexDirection={{ xs: "column", sm: "row" }}
            mb={2}
          ></SuiBox>
          <AlertForm item={item} onSave={onSave} onClose={onClose} />
        </SuiBox>
      </SuiBox>
    </Card>
  );
}

// Setting default values for the props of Bill
SelectedAlert.defaultProps = {};

// Typechecking props for the Bill
SelectedAlert.propTypes = {
  item: PropTypes.object.isRequired,
};

export default SelectedAlert;
