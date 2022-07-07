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
import Grid from "@mui/material/Grid";
import Icon from "@mui/material/Icon";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

// Images

import {format} from "date-fns"

function PositionStatus({item}) {

  return (
      <Card sx={{ height: "100%" }}>
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={2}>
              <SuiTypography variant="h6" fontWeight="medium" textTransform="capitalize">
                  Position Status
              </SuiTypography>
              <SuiBox display="flex" alignItems="flex-start">
                  <SuiTypography variant="button" color="text" fontWeight="regular">
                      {format(new Date(item.timeStamp), "dd/MM/yyy HH:mm:ss")}
                  </SuiTypography>
              </SuiBox>
          </SuiBox>
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={2}>
              <SuiTypography variant="h5" fontWeight="bold" gutterBottom color="info">
                  {item.name}
              </SuiTypography>
          </SuiBox>

          <SuiBox pt={3} pb={2} px={2}>
              <SuiBox mb={2}>
                  <SuiTypography variant="caption" color="text" fontWeight="bold" textTransform="uppercase">
                      Inbound
                  </SuiTypography>
              </SuiBox>
              <SuiBox
                  overflow="auto"
                  height="275px"
                  component="ul"
                  display="flex"
                  flexDirection="column"
                  p={0}
                  m={0}
                  sx={{ listStyle: "none" }}
              >
                  {
                      item.in && item.in.map(p =>
                          (
                              <SuiBox key={p}>
                              <SuiTypography variant="caption" fontWeight="regular">{p}</SuiTypography>
                              </SuiBox>
                          )
                      )}
              </SuiBox>
              <SuiBox mt={1} mb={2}>
                  <SuiTypography variant="caption" color="text" fontWeight="bold" textTransform="uppercase">
                      Outbound
                  </SuiTypography>
              </SuiBox>
              <SuiBox
                  overflow="auto"
                  height="275px"
                  component="ul"
                  display="flex"
                  flexDirection="column"
                  p={0}
                  m={0}
                  sx={{ listStyle: "none" }}
              >
                  {
                      item.out && item.out.map(p =>
                          (
                              <SuiBox key={p}>
                                  <SuiTypography variant="caption" fontWeight="regular">{p}</SuiTypography>
                              </SuiBox>
                          )
                      )}
              </SuiBox>
          </SuiBox>
      </Card>
  );
}

export default PositionStatus;
