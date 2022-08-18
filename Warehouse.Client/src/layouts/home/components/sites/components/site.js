import SuiBox from "components/SuiBox";
import TabOutlinedIcon from "@mui/icons-material/TabOutlined";
import SuiTypography from "components/SuiTypography";
import * as React from "react";

function Site({ site, count }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <TabOutlinedIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium" color={"info"}>
          {site.name || "Undefined"}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {count}&nbsp;items
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}
export default Site;
