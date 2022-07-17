/**
 =========================================================
 * Soft UI Dashboard React - v3.1.0
 =========================================================

 * Product Page: https://www.creative-tim.com/product/soft-ui-dashboard-react
 * Copyright 2022 Creative Tim (https://www.creative-tim.com)

 Coded by www.creative-tim.com

 =========================================================

 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

// @mui material components
import Card from "@mui/material/Card";
import Icon from "@mui/material/Icon";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

// Images
import bg from "assets/images/637499715290000000.jpg";
import SensorsOutlinedIcon from "@mui/icons-material/SensorsOutlined";
import PropTypes from "prop-types";

function Beacon({ macAddress = "n/a", name = "n/a" }) {
  return (
    <Card sx={{ height: "100%" }}>
      <SuiBox position="relative" height="100%" p={2} minHeight="285px">
        <SuiBox
          display="flex"
          flexDirection="column"
          height="100%"
          py={2}
          px={2}
          borderRadius="lg"
          sx={{
            backgroundImage: ({ functions: { linearGradient, rgba }, palette: { gradients } }) =>
              `${linearGradient(
                rgba(gradients.dark.main, 0.6),
                rgba(gradients.dark.state, 0.6)
              )}, url(${bg})`,
            backgroundSize: "cover",
          }}
        >
          <SuiBox mb={3} pt={1} color="white" lineHeight={0} display="flex" alignItems="center">
            <SensorsOutlinedIcon />
            <SuiTypography variant="h5" color="white" fontWeight="bold" px={1}>
              {macAddress}
            </SuiTypography>
          </SuiBox>
          <SuiBox mb={2}>
            <SuiTypography variant="body2" color="white">
              {name}
            </SuiTypography>
          </SuiBox>
          <SuiTypography
            component="a"
            href="#"
            variant="button"
            color="white"
            fontWeight="medium"
            sx={{
              mt: "auto",
              mr: "auto",
              display: "inline-flex",
              alignItems: "center",
              cursor: "pointer",

              "& .material-icons-round": {
                fontSize: "1.125rem",
                transform: `translate(2px, -0.5px)`,
                transition: "transform 0.2s cubic-bezier(0.34,1.61,0.7,1.3)",
              },

              "&:hover .material-icons-round, &:focus  .material-icons-round": {
                transform: `translate(6px, -0.5px)`,
              },
            }}
          >
            View More
            <Icon sx={{ fontWeight: "bold" }}>arrow_forward</Icon>
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    </Card>
  );
}

export default Beacon;

Beacon.defaultProps = {
  macAddress: PropTypes.string.isNotNull,
};
