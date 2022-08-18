import SuiBox from "components/SuiBox";
import QrCode2SharpIcon from "@mui/icons-material/QrCode2Sharp";
import SuiTypography from "components/SuiTypography";
import * as React from "react";

function Product({ product, count }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <QrCode2SharpIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium" color={"primary"}>
          {product.name || "Undefined"}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {count}&nbsp;items
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}
export default Product;
