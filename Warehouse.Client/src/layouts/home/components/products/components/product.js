import SuiBox from "components/SuiBox";
import QrCode2SharpIcon from "@mui/icons-material/QrCode2Sharp";
import SuiTypography from "components/SuiTypography";
import * as React from "react";

function Product({ name, sites }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <QrCode2SharpIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium" color={"primary"}>
          {name}
        </SuiTypography>
        <SuiTypography variant="caption" color="info">
          {sites.length}&nbsp;sites
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}
export default Product;
