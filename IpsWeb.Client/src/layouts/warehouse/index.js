import { useState } from "react";
// @mui material components
import Card from "@mui/material/Card";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

/* eslint-disable react/prop-types */
import SuiAvatar from "components/SuiAvatar";
import SuiBadge from "components/SuiBadge";

// Soft UI Dashboard React examples
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import Table from "examples/Tables/Table";

import { useQuery } from "react-query";
import { client } from "utils/api-client";
import * as auth from "auth-provider";

import WarehouseCanvas from "./conponents/warehouse-canvas";
import CanvasDemo from "./conponents/canvas-demo";

const Warehouse = () => {
  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3}>
        <SuiBox mb={3}>
          <Card>
            <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
              <SuiTypography variant="h6">Warehouse</SuiTypography>
            </SuiBox>
            <SuiBox
              sx={{
                "& .MuiTableRow-root:not(:last-child)": {
                  "& td": {
                    borderBottom: ({ borders: { borderWidth, borderColor } }) =>
                      `${borderWidth[1]} solid ${borderColor}`,
                  },
                },
              }}
            >
                
                <CanvasDemo/>
            </SuiBox>
          </Card>
        </SuiBox>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
};

export default Warehouse;